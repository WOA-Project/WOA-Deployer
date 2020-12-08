using System;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
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
            throw new ApplicationException($"Script interrupted: {reason}");
        }
    }
}
