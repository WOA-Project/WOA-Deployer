using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class Fail : DeployerFunction
    {
        public Fail(IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
        }

        public Task Execute(string reason)
        {
            throw new TaskCanceledException($"Script interrupted: {reason}");
        }
    }
}
