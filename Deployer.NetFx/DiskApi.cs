using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.FileSystem;
using Deployer.FileSystem.Gpt;
using Serilog;
using Zafiro.Core;
using Partition = Deployer.FileSystem.Partition;

namespace Deployer.NetFx
{
    public class DiskApi : IDiskApi
    {
        private readonly PowerShell ps = PowerShell.Create();

        public async Task<List<Disk>> GetDisks()
        {
            var results = await ps.ExecuteScript("Get-Disk");

            var disks = results
                .Select(x => x.ImmediateBaseObject)
                .Select(ToDisk);

            return disks.ToList();
        }

        public async Task<List<Partition>> GetPartitions(Disk disk)
        {
            Log.Debug("Getting partitions from disk {Disk}", disk);
            
            using (var transaction = await GptContextFactory.Create(disk.Number, FileAccess.Read, GptContext.DefaultBytesPerSector, GptContext.DefaultChunkSize))
            {
                var partitions = transaction.Partitions.Select(x => x.AsCommon(disk));
                return partitions.ToList();
            }
        }

        public async Task Format(Partition partition, FileSystemFormat fileSystemFormat, string label = null)
        {
            label = label ?? partition.Name ?? "";

            Log.Verbose(@"Formatting {Partition} as {Format} labeled as ""{Label}""", partition, fileSystemFormat, label);

            var part = await GetPsPartition(partition);
            
            await ps.ExecuteCommand("Format-Volume",
                ("Partition", part),
                ("Force", null),
                ("Confirm", false),
                ("FileSystem", fileSystemFormat.Moniker),
                ("NewFileSystemLabel", label)
            );
        }

        private async Task<PSObject> GetPsPartition(Partition partition)
        {
            var psDataCollection = await ps.ExecuteScript($"Get-Partition -DiskNumber {partition.Disk.Number} | where -Property Guid -eq '{{{partition.Guid}}}'");
            return psDataCollection.First();
        }

        public async Task<IList<Volume>> GetVolumes(Disk disk)
        {
            Log.Debug("Getting volumes from disk {Disk}", disk);

            var partitions = await GetPartitions(disk);

            if (!partitions.Any())
            {
                Log.Verbose("Couldn't find any partition in {Disk}. Updating Storage Cache...", disk);
                await UpdateStorageCache();
                Log.Verbose("Retrying Get Partitions...", disk);
                partitions = await GetPartitions(disk);
            }

            var partitionsObs = partitions.ToObservable();

            var volumes = partitionsObs
                .Select(x => Observable.FromAsync(async () =>
                {
                    try
                    {
                        return await GetVolume(x);
                    }
                    catch (Exception)
                    {
                        Log.Warning($"Cannot get volume for {x}");
                        return null;
                    }
                }))
                .Merge(1)
                .Where(v => v != null)
                .ToList();

            return await volumes;
        }

        public async Task RemovePartition(Partition partition)
        {
            Log.Verbose("Removing {Partition}", partition);

            using (var c = await  GptContextFactory.Create(partition.Disk.Number, FileAccess.ReadWrite, GptContext.DefaultBytesPerSector, GptContext.DefaultChunkSize))
            {
                var gptPart = c.Find(partition.Guid);
                c.Delete(gptPart);
            }

            await partition.Disk.Refresh();
        }

        public async Task RefreshDisk(Disk disk)
        {
            Log.Verbose("Refreshing {Disk}. Partitions before refresh:\n{Partitions}", disk, await GetListOfPartitions(disk));

            var script = $"SELECT DISK {disk.Number}\nOFFLINE DISK\nONLINE DISK";
            await ps.ExecuteScript($@"""{script}"" | & diskpart.exe");

            Log.Verbose("{Disk} refreshed. Partitions after refresh:\n{Partitions}", disk, await GetListOfPartitions(disk));
        }

        private static async Task<string> GetListOfPartitions(Disk disk)
        {
            var partitions = await disk.GetPartitions();
            var str = partitions.AsNumberedList();
             return str;
        }

        public char GetFreeDriveLetter()
        {
            Log.Debug("Getting free drive letter");
            
            var drives = Enumerable.Range('C', 'Z').Select(i => (char)i);
            var usedDrives = DriveInfo.GetDrives().Select(x => char.ToUpper(x.Name[0]));

            var available = drives.Except(usedDrives);

            return available.First();
        }


        public async Task AssignDriveLetter(Partition partition, char driveLetter)
        {
            Log.Debug("Assigning drive letter {Letter} to {Partition}", driveLetter, partition);

            var psPart = await GetPsPartition(partition);
            await ps.ExecuteCommand("Set-Partition", 
                ("InputObject", psPart),
                ("NewDriveLetter", driveLetter));

            Log.Debug("Drive letter assigned");
        }

        public async Task SetGptType(Partition partition, PartitionType partitionType)
        {
            Log.Verbose("Setting new GPT partition type {Type} to {Partition}", partitionType, partition);

            if (Equals(partition.PartitionType, partitionType))
            {
                return;
            }

            using (var context = await GptContextFactory.Create(partition.Disk.Number, FileAccess.ReadWrite, GptContext.DefaultBytesPerSector, GptContext.DefaultChunkSize))
            {
                var part = context.Find(partition.Guid);
                part.PartitionType = partitionType;
            }            

            await partition.Disk.Refresh();

            Log.Verbose("New GPT type set correctly", partitionType, partition);

        }

        public async Task<Volume> GetVolume(Partition partition)
        {
            Log.Debug("Getting volume of {Partition}", partition);

            var results = await ps.ExecuteCommand("Get-Volume", 
                ("Partition", await GetPsPartition(partition)));

            var result = results.FirstOrDefault()?.ImmediateBaseObject;

            if (result == null)
            {
                return null;
            }

            var vol = new Volume(partition)
            {
                Partition = partition,
                Size = new ByteSize(Convert.ToUInt64(result.GetPropertyValue("Size"))),
                Label = (string)result.GetPropertyValue("FileSystemLabel"),
                Letter = (char?)result.GetPropertyValue("DriveLetter")
            };

            Log.Debug("Obtained {Volume}", vol);
            
            return vol;
        }

        public async Task ResizePartition(Partition partition, ByteSize size)
        {
            if (size.MegaBytes < 0)
            {
                throw new InvalidOperationException($"The partition size cannot be negative: {size}");
            }

            var sizeBytes = (ulong)size.Bytes;
            Log.Verbose("Resizing partition {Partition} to {Size}", partition, size);

            var psPart = await GetPsPartition(partition);
            await ps.ExecuteCommand("Resize-Partition", 
                ("InputObject", psPart),
                ("Size", sizeBytes));

            if (ps.HadErrors)
            {
                Throw("The resize operation has failed");
            }
        }

        private void Throw(string message)
        {
            var errors = string.Join(",", ps.Streams.Error.ReadAll());

            var invalidOperationException = new InvalidOperationException($@"{message}. Details: {errors}");
            Log.Error(invalidOperationException, message);

            throw invalidOperationException;
        }

        public async Task<ICollection<DriverMetadata>> GetDrivers(Volume volume)
        {
            var results = await ps.ExecuteScript($"Get-WindowsDriver -Path {volume.Root}");

            var disks = results
                .Select(ToDriverMetadata);

            return disks.ToList();
        }

        private static DriverMetadata ToDriverMetadata(PSObject driverMetadata)
        {
            return new DriverMetadata
            {
                Driver = (string)driverMetadata.Properties["Driver"].Value,
                OriginalFileName = (string)driverMetadata.Properties["OriginalFileName"].Value,
                Inbox = (bool)driverMetadata.Properties["Inbox"].Value,
                BootCritical = (bool)driverMetadata.Properties["BootCritical"].Value,
                ProviderName = (string)driverMetadata.Properties["ProviderName"].Value,
                Date = (DateTime)driverMetadata.Properties["Date"].Value,
            };
        }

        public async Task ChangeDiskId(Disk disk, Guid guid)
        {
            Log.Verbose("Changing disk Guid {Guid} to {Disk}", guid, disk);

            var cmd = $@"Set-Disk -Number {disk.Number} -Guid ""{{{guid}}}""";
            await ps.ExecuteScript(cmd);

            if (ps.HadErrors)
            {
                Throw($"Cannot set the Guid {guid} to the disk {disk}");
            }

            Log.Verbose("Disk Guid changed", guid, disk);
        }

        public Task UpdateStorageCache()
        {
            return ps.ExecuteScript("Update-HostStorageCache");
        }

        public async Task<Disk> GetDisk(int n)
        {
            Log.Verbose("Getting disk by index {Id}", n);
            var results = await ps.ExecuteScript($"Get-Disk -Number {n}");

            var disks = results
                .Select(x => x.ImmediateBaseObject)
                .Select(ToDisk);

            var disk = disks.First();

            Log.Verbose("Returned disk {Disk}", disk);

            return disk;
        }

        private Disk ToDisk(object disk)
        {
            var number = (uint)disk.GetPropertyValue("Number");
            var size = new ByteSize((ulong)disk.GetPropertyValue("Size"));
            var allocatedSize = new ByteSize((ulong)disk.GetPropertyValue("AllocatedSize"));

            var diskInfo = new DiskInfo
            {
                Number = number,
                Size = size,
                AllocatedSize = allocatedSize,
                FriendlyName = (string)disk.GetPropertyValue("FriendlyName"),
                IsSystem = (bool)disk.GetPropertyValue("IsSystem"),
                IsBoot = (bool)disk.GetPropertyValue("IsBoot"),
                IsOffline = (bool)disk.GetPropertyValue("IsOffline"),
                IsReadOnly = (bool)disk.GetPropertyValue("IsReadOnly")
            };

            return new Disk(this, diskInfo);
        }
    }
}