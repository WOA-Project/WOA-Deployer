using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions.Partitions
{
    public class Format : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public Format(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations,
             IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(string partitionDescriptor, string fileSystemFormat, string label = null)
        {
            var part = await fileSystem.TryGetPartitionFromDescriptor(partitionDescriptor);
            await part.DoAsync(async (p, ct) =>
            {
                await p.EnsureWritable();
                var vol = await p.GetVolume();
                var systemFormat = FileSystemFormat.FromString(fileSystemFormat);
                await vol.Format(systemFormat, label);
            });
        }
    }
}