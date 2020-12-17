using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Deployer.Filesystem.Gpt;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Functions.Partitions
{
    public class RemoveGptPartitions : DeployerFunction
    {
        private readonly IFileSystem fileSystem;

        public RemoveGptPartitions( IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext, IFileSystem fileSystem) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(int diskNumber, string namesList)
        {
            using (var context = await GptContextFactory.Create((uint) diskNumber, FileAccess.ReadWrite))
            {
                foreach (var name in namesList.Split(';'))
                {
                    context.RemoveExisting(name);
                }
            }

            var disk = await fileSystem.GetDisk(diskNumber);
            await disk.Refresh();
        }
    }
}