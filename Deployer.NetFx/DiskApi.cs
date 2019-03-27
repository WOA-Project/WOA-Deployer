﻿using System;
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

        public Task<List<Partition>> GetPartitions(Disk disk)
        {
            using (var transaction = new GptContext(disk.Number, FileAccess.Read))
            {
                var partitions = transaction.Partitions.Select(x => x.AsCommon(disk));
                return Task.FromResult(partitions.ToList());
            }
        }

        public async Task Format(Partition partition, FileSystemFormat fileSystemFormat, string label = null)
        {
            var part = await GetPsPartition(partition);
            
            await ps.ExecuteCommand("Format-Volume",
                ("Partition", part),
                ("Force", null),
                ("Confirm", false),
                ("FileSystem", fileSystemFormat.Moniker),
                ("NewFileSystemLabel", label ?? partition.Name ?? "")
            );
        }

        private async Task<PSObject> GetPsPartition(Partition partition)
        {
            var psDataCollection = await ps.ExecuteScript($"Get-Partition -DiskNumber {partition.Disk.Number} | where -Property Guid -eq '{{{partition.Guid}}}'");
            return psDataCollection.First();
        }

        public async Task<IList<Volume>> GetVolumes(Disk disk)
        {
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
                        Log.Warning($"Cannot get volume for partition {x}");
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
            using (var c = new GptContext(partition.Disk.Number, FileAccess.ReadWrite))
            {
                var gptPart = c.Find(partition.Guid);
                c.Delete(gptPart);
            }

            await partition.Disk.Refresh();
        }

        public Task RefreshDisk(Disk disk)
        {
            var script = $"SELECT DISK {disk.Number}\nOFFLINE DISK\nONLINE DISK";
            return ps.ExecuteScript($@"""{script}"" | & diskpart.exe");
        }

        public char GetFreeDriveLetter()
        {
            var drives = Enumerable.Range('C', 'Z').Select(i => (char)i);
            var usedDrives = DriveInfo.GetDrives().Select(x => char.ToUpper(x.Name[0]));

            var available = drives.Except(usedDrives);

            return available.First();
        }


        public async Task AssignDriveLetter(Partition partition, char driveLetter)
        {
            var psPart = await GetPsPartition(partition);
            await ps.ExecuteCommand("Set-Partition", 
                ("InputObject", psPart),
                ("NewDriveLetter", driveLetter));
        }

        public async Task SetGptType(Partition partition, PartitionType partitionType)
        {
            if (Equals(partition.PartitionType, partitionType))
            {
                return;
            }

            using (var context = new GptContext(partition.Disk.Number, FileAccess.ReadWrite))
            {
                var part = context.Partitions.Single(x => x.Number == partition.Number);
                part.PartitionType = partitionType;
            }            

            await partition.Disk.Refresh();
        }

        public async Task<Volume> GetVolume(Partition partition)
        {
            var results = await ps.ExecuteCommand("Get-Volume", 
                ("Partition", await GetPsPartition(partition)));

            var volume = results.FirstOrDefault()?.ImmediateBaseObject;

            if (volume == null)
            {
                return null;
            }

            return new Volume(partition)
            {
                Partition = partition,
                Size = new ByteSize(Convert.ToUInt64(volume.GetPropertyValue("Size"))),
                Label = (string)volume.GetPropertyValue("FileSystemLabel"),
                Letter = (char?)volume.GetPropertyValue("DriveLetter")
            };
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

        public Task ChangeDiskId(Disk disk, Guid guid)
        {
            var cmd = $@"Set-Disk -Number {disk.Number} -Guid ""{{{guid}}}""";
            return ps.ExecuteScript(cmd);
        }

        public Task UpdateStorageCache()
        {
            return ps.ExecuteScript("Update-HostStorageCache");
        }

        public async Task<Disk> GetDisk(int i)
        {
            var results = await ps.ExecuteScript($"Get-Disk -Number {i}");

            var disks = results
                .Select(x => x.ImmediateBaseObject)
                .Select(ToDisk);

            return disks.First();
        }

        public async Task SetGuid(Disk disk, Guid guid)
        {
            var cmd = $@"Set-Disk -Number {disk.Number} -Guid ""{{{guid}}}""";
            await ps.ExecuteScript(cmd);

            if (ps.HadErrors)
            {
                Throw($"Cannot set the Guid {guid} to the disk {disk}");
            }
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