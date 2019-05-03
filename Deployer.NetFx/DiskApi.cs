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
        public async Task<List<Disk>> GetDisks()
        {
            var results = await PowerShellMixin.ExecuteScript("Get-Disk");

            var disks = results
                .Select(x => x.ImmediateBaseObject)
                .Select(ToDisk);

            return disks.ToList();
        }

        public async Task<List<Partition>> GetPartitions(Disk disk)
        {
            Log.Debug("Getting partitions from disk {Disk}", disk);
            
            using (var transaction = await GptContextFactory.Create(disk.Number, FileAccess.Read))
            {
                var partitions = transaction.Partitions.Select(x => x.AsCommon(disk));
                return partitions.ToList();
            }
        }

        public async Task Format(Partition partition, FileSystemFormat fileSystemFormat, string label = null)
        {
            await Observable.FromAsync(() => FormatCore(partition, fileSystemFormat, label)).RetryWithBackoffStrategy(4);
        }

        private async Task FormatCore(Partition partition, FileSystemFormat fileSystemFormat, string label = null)
        {
            label = label ?? partition.Name ?? "";

            Log.Verbose(@"Formatting {Partition} as {Format} labeled as ""{Label}""", partition, fileSystemFormat, label);

            var part = await GetPsPartition(partition);
            
            await PowerShellMixin.ExecuteCommand("Format-Volume",
                ("Partition", part),
                ("Force", null),
                ("Confirm", false),
                ("FileSystem", fileSystemFormat.Moniker),
                ("NewFileSystemLabel", label)
            );

            await EnsureFormatted(partition, label, fileSystemFormat);
        }

        private static async Task EnsureFormatted(Partition partition, string label, FileSystemFormat fileSystemFormat)
        {
            var volume = await partition.GetVolume();
            if (volume == null)
            {
                throw new ApplicationException($"Couldn't get volume for {partition}");
            }

            await volume.Mount();

            if (Directory.EnumerateFileSystemEntries(volume.Root).Count() > 1)
            {
                throw new ApplicationException("The format operation failed. The drive shouldn't contain any file after the format"); 
            }

            var sameLabel = string.Equals(volume.Label, label);
            var sameFileSystemFormat = Equals(volume.FileSytemFormat, fileSystemFormat);
            if (!sameLabel || !sameFileSystemFormat)
            {
                Log.Verbose("Same label? {Value}. Same file system format? {Value}", sameLabel, sameFileSystemFormat);
                throw new ApplicationException("The format operation failed");
            }
        }

        private async Task<PSObject> GetPsPartition(Partition partition)
        {
            var psDataCollection = await PowerShellMixin.ExecuteScript($"Get-Partition -DiskNumber {partition.Disk.Number} | where -Property Guid -eq '{{{partition.Guid}}}'");
            var psPartition = psDataCollection.FirstOrDefault();

            if (psPartition == null)
            {
                await partition.Disk.Refresh();
                throw new ApplicationException($"Could not get PS Partition for {partition}");
            }

            return psPartition;
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

            using (var c = await  GptContextFactory.Create(partition.Disk.Number, FileAccess.ReadWrite))
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
            await PowerShellMixin.ExecuteScript($@"""{script}"" | & diskpart.exe");

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
            await Observable
                .Defer(() => Observable
                    .FromAsync(() => ChangeDriveLetterCore(partition, driveLetter)))
                .RetryWithBackoffStrategy(5);
            Log.Debug("{Partition} mounted successfully as driver letter {Letter}", partition, driveLetter);
        }

        private async Task ChangeDriveLetterCore(Partition partition, char driveLetter)
        {
            Log.Debug("Assigning drive letter {Letter} to {Partition}", driveLetter, partition);

            var psPart = await GetPsPartition(partition);
            await PowerShellMixin.ExecuteCommand("Set-Partition", 
                ("InputObject", psPart),
                ("NewDriveLetter", driveLetter));

            Log.Debug("Ensuring the volume is mounted...");
            EnsurePathExists($"{driveLetter}:\\");
        }

        private static void EnsurePathExists(string path)
        {
            Log.Debug("Checking path {Path}", path);
            if (!Directory.Exists(path))
            {
                throw new ApplicationException($"The path '{path}' isn't ready yet");
            }

            Log.Debug("The path {Path} is ready", path);
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
            return await Observable.FromAsync( () => GetVolumeCore(partition)).RetryWithBackoffStrategy(4);
        }

        private async Task<Volume> GetVolumeCore(Partition partition)
        {
            Log.Debug("Getting volume of {Partition}", partition);

            var results = await PowerShellMixin.ExecuteCommand("Get-Volume",
                ("Partition", await GetPsPartition(partition)));

            var result = results.FirstOrDefault()?.ImmediateBaseObject;

            if (result == null)
            {
                throw new ApplicationException($"Cannot get volume for {partition}");
            }

            var vol = new Volume(partition)
            {
                Partition = partition,
                Size = new ByteSize(Convert.ToUInt64(result.GetPropertyValue("Size"))),
                Label = (string) result.GetPropertyValue("FileSystemLabel"),
                Letter = (char?) result.GetPropertyValue("DriveLetter"),
                FileSytemFormat = FileSystemFormat.FromString((string) result.GetPropertyValue("FileSystem")),
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
            await PowerShellMixin.ExecuteCommand("Resize-Partition", 
                ("InputObject", psPart),
                ("Size", sizeBytes));
        }

        public async Task<ICollection<DriverMetadata>> GetDrivers(Volume volume)
        {
            var results = await PowerShellMixin.ExecuteScript($"Get-WindowsDriver -Path {volume.Root}");

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
            await PowerShellMixin.ExecuteScript(cmd);

            Log.Verbose("Disk Guid changed", guid, disk);
        }

        public Task UpdateStorageCache()
        {
            return PowerShellMixin.ExecuteScript("Update-HostStorageCache");
        }

        public async Task<Disk> GetDisk(int n)
        {
            Log.Verbose("Getting disk by index {Id}", n);
            var results = await PowerShellMixin.ExecuteScript($"Get-Disk -Number {n}");

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
                IsReadOnly = (bool)disk.GetPropertyValue("IsReadOnly"),
                UniqueId = (string)disk.GetPropertyValue("UniqueId"),
            };

            return new Disk(this, diskInfo);
        }
    }
}