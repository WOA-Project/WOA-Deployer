using System;
using System.Threading.Tasks;
using Deployer.Core.Scripting.MicroParser;
using Deployer.Filesystem;
using Optional;

namespace Deployer.Functions.Partitions
{
    public static class PartitionConversionMixin 
    {
        public static async Task<Option<IPartition>> TryGetPartitionFromDescriptor(this IFileSystem fileSystem, string descriptor)
        {
            var mini = Parser.Parse(descriptor);
            var diskNumber = (int?)mini["Disk"];

            if (diskNumber == null)
            {
                throw new InvalidOperationException($"Disk value is missing while parsing partition descriptor: {descriptor}");
            }

            var label = (string)mini["Label"];
            var name = (string)mini["Name"];
            var number = (int?)mini["Number"];

            var disk = await fileSystem.GetDisk(diskNumber.Value);
            IPartition part = null;

            if (number.HasValue)
            {
                part = await disk.GetPartitionByNumber(number.Value);
            }

            if (part is null && !(label is null))
            {
                part = await disk.GetPartitionByVolumeLabel(label);
            }

            if (part is null && name != null)
            {
                part = await disk.GetPartitionByName(name);
            }

            if (part != null)
            {
                return part.Some();
            }

            return Option.None<IPartition>();
        }
    }
}