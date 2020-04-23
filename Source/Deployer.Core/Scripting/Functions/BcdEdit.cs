using System;
using System.Threading.Tasks;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting.Functions
{
    public class BcdEdit: DeployerFunction
    {
        private readonly Func<string, IBcdInvoker> invokerFactory;

        public BcdEdit(IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext, Func<string, IBcdInvoker> invokerFactory) : base(fileSystemOperations, operationContext)
        {
            this.invokerFactory = invokerFactory;
        }

        public Task Execute(string store, string command)
        {
            return invokerFactory(store).Invoke(command);
        }
    }
}