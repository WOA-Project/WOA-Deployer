using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;
using Serilog;
using Zafiro.Core;

namespace Deployer.NetFx
{
    public class Disk : IDisk
    {
        public Disk(DiskInfo diskProps)
        {
            FriendlyName = diskProps.FriendlyName;
            Number = diskProps.Number;
            Size = diskProps.Size;
            AllocatedSize = diskProps.AllocatedSize;
            FriendlyName = diskProps.FriendlyName;
            IsSystem = diskProps.IsSystem;
            IsBoot = diskProps.IsBoot;
            IsReadOnly = diskProps.IsReadOnly;
            IsOffline = diskProps.IsOffline;
            UniqueId = diskProps.UniqueId;
        }

        public ByteSize Size { get; }

        public bool IsBoot { get; }

        public bool IsReadOnly { get; }

        public bool IsOffline { get; }

        public bool IsSystem { get; }

        public ByteSize AllocatedSize { get; }

        public string FriendlyName { get; }

        public uint Number { get; }
        public ByteSize AvailableSize => Size - AllocatedSize;
        public string UniqueId { get; }

        public async Task<IList<IPartition>> GetPartitions()
        {
            var results = await PowerShellMixin.ExecuteScript($"Get-Partition -DiskNumber {Number}");

            var wmiPartitions = results
                .Select(x => x.ImmediateBaseObject)
                .Select(x => ToWmiPartition(x));

            ReadOnlyCollection<FileSystem.Gpt.Partition> gptPartitions;
            using (var context = await GptContextFactory.Create(Number, FileAccess.Read))
            {
                gptPartitions = context.Partitions;
            }

            var partitions = wmiPartitions
                .Join(gptPartitions, x => x.Guid, x => x.Guid, (wmi, gpt) => (IPartition)new Partition(this)
            {
                Name = gpt.Name,
                Root = wmi.Root,
                Number = wmi.Number,
                Guid = wmi.Guid,
                PartitionType = wmi.PartitionType,
                UniqueId = wmi.UniqueId,
                Size = wmi.Size,
            });
            return partitions.ToList();
        }

        private static WmiPartition ToWmiPartition(object partition)
        {
            var guid = (string)partition.GetPropertyValue("GptType");
            var partitionType = guid != null ? PartitionType.FromGuid(Guid.Parse(guid)) : null;

            var driveLetter = (char)partition.GetPropertyValue("DriveLetter");

            return new WmiPartition()
            {
                Number = (uint)partition.GetPropertyValue("PartitionNumber"),
                UniqueId = (string)partition.GetPropertyValue("UniqueId"),
                Guid = Guid.Parse((string)partition.GetPropertyValue("Guid")),
                Root = driveLetter != 0 ? PathExtensions.GetRootPath(driveLetter) : null,
                PartitionType = partitionType,
                Size = new ByteSize(Convert.ToUInt64(partition.GetPropertyValue("Size"))),
            };
        }

        public async Task Refresh()
        {
            await PowerShellMixin.ExecuteScript($@"Update-HostStorageCache");
        }

        private static async Task<string> GetListOfPartitions(Disk disk)
        {
            var partitions = await disk.GetPartitions();
            var str = partitions.AsNumberedList();
            return str;
        }

        public async Task SetGuid(Guid guid)
        {
            Log.Verbose("Changing disk Guid {Guid} to {Disk}", guid, this);
            var cmd = $@"Set-Disk -Number {Number} -Guid ""{{{guid}}}""";
            await PowerShellMixin.ExecuteScript(cmd);
            
            Log.Verbose("Disk Guid changed", guid, this);
        }

        public async Task ToggleOnline(bool isOnline)
        {
            await PowerShellMixin.ExecuteCommand("Set-Disk",
                ("Number", Number),
                ("IsOffline", !isOnline));
        }

        public override string ToString()
        {
            return $"Disk {Number} ({FriendlyName})";
        }

        public async Task<IPartition> CreatePartition(ByteSize size, PartitionType partitionType, string name = "")
        {
            if (size.Equals(ByteSize.MaxValue))
            {
                await PowerShellMixin.ExecuteScript($"New-Partition -DiskNumber {Number} -UseMaximumSize");
            }
            else
            {
                await PowerShellMixin.ExecuteScript($"New-Partition -DiskNumber {Number} -Size {size.Bytes}");
            }

            var partitions = await GetPartitions();
            return partitions.Last();
        }
    }

    internal class WmiPartition
    {
        public uint Number { get; set; }
        public string UniqueId { get; set; }
        public Guid Guid { get; set; }
        public string Root { get; set; }
        public PartitionType PartitionType { get; set; }
        public ByteSize Size { get; set; }
    }
}