using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class Copy : DeployerFunction
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public Copy(IFileSystemOperations fileSystemOperations,
             IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task Execute(string origin, string destination)
        {
            OperationContext.CancellationToken.ThrowIfCancellationRequested();
            await fileSystemOperations.Copy(origin, destination);
        }
    }
}