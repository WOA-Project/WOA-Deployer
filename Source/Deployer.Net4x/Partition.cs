using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Filesystem;
using Serilog;

namespace Deployer.Net4x
{
    public class Partition : IPartition
    {
        public Partition(IDisk disk)
        {
            Disk = disk;
        }

        public IDisk Disk { get; }

        public Guid? Guid { get; set; }
        public GptType GptType { get; set; }
        public string Root { get; set; }
        public string Name { get; set; }
        public uint Number { get; set; }
        public string UniqueId { get; set; }

        public async Task<char> AssignDriveLetter()
        {
            var part = await this.GetPsPartition();
            var letter = GetFreeDriveLetter();
            await PowerShellMixin.ExecuteCommand("Set-Partition",
                ("InputObject", part),
                ("NewDriveLetter", letter)
            );

            Root = PathExtensions.GetRootPath(letter);

            return letter;
        }

        public ByteSize Size { get; set; }

        private static char GetFreeDriveLetter()
        {
            Log.Debug("Getting free drive letter");

            var drives = Enumerable.Range('C', 'Z').Select(i => (char)i);
            var usedDrives = DriveInfo.GetDrives().Select(x => char.ToUpper(x.Name[0]));

            var available = drives.Except(usedDrives);

            var driveLetter = available.First();

            Log.Verbose("Free drive letter={Letter}", driveLetter);

            return driveLetter;
        }

        public async Task SetGptType(GptType gptType)
        {
            Log.Verbose("Setting new GPT partition type {Type} to {Partition}", gptType, this);

            if (Equals(GptType, gptType))
            {
                return;
            }

            var part = await this.GetPsPartition();
            await PowerShellMixin.ExecuteCommand("Set-Partition",
                ("InputObject", part),
                ("GptType", $"{{{gptType.Guid}}}")
            );
            await Disk.Refresh();

            Log.Verbose("New GPT type set correctly", gptType, this);
        }

        public async Task<IVolume> GetVolume()
        {
            Log.Debug("Getting volume of {Partition}", this);

            var results = await PowerShellMixin.ExecuteCommand("Get-Volume",
                ("Partition", await this.GetPsPartition()));

            var result = results.FirstOrDefault()?.ImmediateBaseObject;

            if (result == null)
            {
                return null;
            }

            var driveLetter = (char?)result.GetPropertyValue("DriveLetter");
            var vol = new Volume(this)
            {
                Size = new ByteSize(Convert.ToUInt64(result.GetPropertyValue("Size"))),
                Label = (string)result.GetPropertyValue("FileSystemLabel"),
                Root = driveLetter != null ? PathExtensions.GetRootPath(driveLetter.Value) : null,
                FileSystemFormat = FileSystemFormat.FromString((string)result.GetPropertyValue("FileSystem")),
            };

            Log.Debug("Obtained {Volume}", vol);

            return vol;
        }

        public async Task Resize(ByteSize size)
        {
            if (size.MegaBytes < 0)
            {
                throw new InvalidOperationException($"The partition size cannot be negative: {size}");
            }

            var sizeBytes = (ulong)size.Bytes;
            Log.Verbose("Resizing partition {Partition} to {Size}", this, size);

            var psPart = await this.GetPsPartition();
            await PowerShellMixin.ExecuteCommand("Resize-Partition", ("InputObject", psPart), ("Size", sizeBytes));
        }

        public async Task RemoveDriveLetter()
        {
            var part = await this.GetPsPartition();
            await PowerShellMixin.ExecuteCommand("Remove-PartitionAccessPath",
                ("InputObject", part),
                ("AccessPath", Root)
            );
        }

        public async Task Remove()
        {
            var part = await this.GetPsPartition();
            await PowerShellMixin.ExecuteCommand("Remove-Partition", ("InputObject", part), ("Confirm", false));
        }

        public override string ToString()
        {
            return $@"Partition '{Name ?? "Unnamed"}' - Guid: {Guid} in {Disk}. ";
        }

        protected bool Equals(IPartition other)
        {
            return Disk.Equals(other.Disk) && Guid.Equals(other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Partition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Disk.GetHashCode() * 397) ^ Guid.GetHashCode();
            }
        }
    }
}