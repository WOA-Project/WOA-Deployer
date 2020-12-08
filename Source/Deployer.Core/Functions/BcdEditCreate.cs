using System;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class BcdEditCreate : DeployerFunction
    {
        private readonly Func<string, IBcdInvoker> invokerFactory;

        public BcdEditCreate(IFileSystemOperations fileSystemOperations, IOperationContext operationContext, Func<string, IBcdInvoker> invokerFactory) : base(fileSystemOperations, operationContext)
        {
            this.invokerFactory = invokerFactory;
        }

        public async Task Execute(string store, string guid, string args)
        {
            var invoker = invokerFactory(store);
            var output = await invoker.Invoke($"/enum {{{guid}}}");
            var alreadyExists = output.Contains("{") && output.Contains("}");

            if (alreadyExists)
            {
                return;
            }

            await invoker.Invoke($"/create {{{guid}}} {args}");
        }
    }
}