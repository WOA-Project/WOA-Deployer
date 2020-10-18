using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleScript;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class ScriptDeployer : Deployer
    {
        public ScriptDeployer(IRunner runner, ICompiler compiler, IRequirementSatisfier requirementSatisfier, IFileSystemOperations fileSystemOperations) : base(runner, compiler, requirementSatisfier, fileSystemOperations)
        {
        }

        public async Task RunScript(string path)
        {
            await Run(path, new Dictionary<string, object>());
            Message("Script execution finished");
        }
    }

    public abstract class BrandNewDeployerBase
    {
        protected void Message(string message)
        {
            //additionalMessages.OnNext(message);
        }
    }
}