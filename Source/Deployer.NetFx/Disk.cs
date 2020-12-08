using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core;
using Deployer.Filesystem;
using Serilog;

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

        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsBoot { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsReadOnly { get; }

        public bool IsOffline { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsSystem { get; }

        public ByteSize AllocatedSize { get; }

        public string FriendlyName { get; }

        public uint Number { get; }
        public ByteSize AvailableSize => Size - AllocatedSize;
        public string UniqueId { get; }

        public async Task<IList<IPartition>> GetPartitions()
        {
            var results = await PowerShellMixin.ExecuteScript($"Get-Partition -DiskNumber {Number}");

            var fromWmi = GetWmiPartitions(results);

            var fromGpt = await GetGptPartitions();

            IPartition FirstSelector(PartitionData wmi) =>
                new Partition(this)
                {
                    Root = wmi.Root,
                    Number = wmi.Number,
                    Guid = wmi.Guid,
                    GptType = wmi.GptType,
                    UniqueId = wmi.UniqueId,
                    Size = wmi.Size,
                };

            IPartition BothSelector(PartitionData wmi, PartitionData gpt) =>
                new Partition(this)
                {
                    Name = gpt.Name,
                    Root = wmi.Root,
                    Number = wmi.Number,
                    Guid = wmi.Guid,
                    GptType = wmi.GptType,
                    UniqueId = wmi.UniqueId,
                    Size = wmi.Size,
                };

            var partitions = MoreLinq.MoreEnumerable.LeftJoin(fromWmi, fromGpt,
                pd => pd.Guid, FirstSelector, BothSelector);

            return partitions.ToList();
        }

        private static IEnumerable<PartitionData> GetWmiPartitions(PSDataCollection<PSObject> results)
        {
            var fromWmi = results
                .Select(x => x.ImmediateBaseObject)
                .Select(ToWmiPartition)
                .Select(wmi => new PartitionData
                {
                    Root = wmi.Root,
                    Number = wmi.Number,
                    Guid = wmi.Guid,
                    GptType = wmi.GptType,
                    UniqueId = wmi.UniqueId,
                    Size = wmi.Size,
                });
            return fromWmi;
        }

        private async Task<IEnumerable<PartitionData>> GetGptPartitions()
        {
            ReadOnlyCollection<Filesystem.Gpt.Partition> gptPartitions;
            using (var context = await GptContextFactory.Create(Number, FileAccess.Read))
            {
                gptPartitions = context.Partitions;
            }

            var fromGpt = gptPartitions.Select(gpt => new PartitionData
                {
                    Name = gpt.Name,
                    Guid = gpt.Guid,
                }
            );
            return fromGpt;
        }

        private static WmiPartition ToWmiPartition(object partition)
        {
            var gptType = (string)partition.GetPropertyValue("GptType");
            var partitionType = gptType != null ? GptType.FromGuid(Guid.Parse(gptType)) : null;

            var driveLetter = (char)partition.GetPropertyValue("DriveLetter");

            return new WmiPartition
            {
                Number = (uint) partition.GetPropertyValue("PartitionNumber"),
                UniqueId = (string) partition.GetPropertyValue("UniqueId"),
                Guid = Guid.TryParse((string) partition.GetPropertyValue("Guid"), out var guid) ? guid : (Guid?) null,
                Root = driveLetter != 0 ? PathExtensions.GetRootPath(driveLetter) : null,
                GptType = partitionType,
                Size = new ByteSize(Convert.ToUInt64(partition.GetPropertyValue("Size"))),
            };
        }

        public async Task Refresh()
        {
            await PowerShellMixin.ExecuteScript($@"Update-HostStorageCache");
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

        public async Task PrepareForRemoval()
        {
            var script = $"SELECT DISK {Number}\nOFFLINE DISK\nONLINE DISK";
            await PowerShellMixin.ExecuteScript($@"""{script}"" | & diskpart.exe");
        }

        public async Task ClearAs(DiskType diskType)
        {
            await PowerShellMixin
                .ExecuteCommand("Clear-Disk",
                    ("RemoveData", null),
                    ("RemoveOEM", null),
                    ("Confirm", false),
                    ("Number", Number));

            await PowerShellMixin
                .ExecuteCommand("Set-Disk",
                    ("PartitionStyle", diskType.ToString().ToUpper()),
                    ("Number", Number));

        }

        public override string ToString()
        {
            return $"Disk {Number} ({FriendlyName})";
        }

        public async Task<IPartition> CreateGptPartition(GptType gptType, ByteSize size = default)
        {
            var sizeStr = size == default ? "-UseMaximumSize" : $"-Size {size.ToString().Replace(" ", "")}";
            await PowerShellMixin.ExecuteScript($"New-Partition -DiskNumber {Number} {sizeStr} -GptType '{{{gptType.Guid}}}'");

            var partitions = await GetPartitions();
            return partitions.Last();
        }

        public async Task<IPartition> CreateMbrPartition(MbrType mbrType, ByteSize size = default)
        {
            var sizeStr = size == default ? "-UseMaximumSize" : $"-Size {size}";
            await PowerShellMixin.ExecuteScript($"New-Partition -DiskNumber {Number} {sizeStr} -MbrType {mbrType.Name}");

            var partitions = await GetPartitions();
            return partitions.Last();
        }
    }
}