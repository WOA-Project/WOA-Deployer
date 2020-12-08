using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class CopyDirectory : DeployerFunction
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public CopyDirectory(IFileSystemOperations fileSystemOperations,
             IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public Task Execute(string origin, string destination)
        {
            return fileSystemOperations.CopyDirectory(origin, destination);
        }
    }
}