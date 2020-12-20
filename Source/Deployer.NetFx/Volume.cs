using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Filesystem;

namespace Deployer.Net4x
{
    public class Volume : IVolume
    {
        public Volume(IPartition partition)
        {
            Partition = partition;
        }

        public IPartition Partition { get; }
        public ByteSize Size { get; set; }
        public string Label { get; set; }
        public FileSystemFormat FileSystemFormat { get; set; }
        public string Root { get; set; }

        public async Task<ICollection<DriverMetadata>> GetDrivers()
        {
            var results = await PowerShellMixin.ExecuteScript($"Get-WindowsDriver -Path {Root}");

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

        public async Task Format(FileSystemFormat fileSystemFormat, string label)
        {
            var part = await this.Partition.GetPsPartition();
            await PowerShellMixin.ExecuteCommand("Format-Volume",
                          ("Partition", part),
                          ("Force", null),
                          ("Confirm", false),
                          ("FileSystem", fileSystemFormat.Moniker),
                          ("NewFileSystemLabel", label)
                      );
        }

        public override string ToString()
        {
            var label = Label ?? "No label";
            return $"Volume '{label}' at {Partition} {FileSystemFormat.Moniker}";
        }
    }
}