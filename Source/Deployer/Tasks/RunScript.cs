using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;
using Serilog;

namespace Deployer.Tasks
{
    [TaskDescription("Running script '{0}'")]
    public class RunScript : DeploymentTask
    {
        private readonly string scriptPath;
        private readonly IScriptRunner scriptRunner;
        private readonly IScriptParser scriptParser;

        public RunScript(string scriptPath, IFileSystemOperations fileSystemOperations,
            IDeploymentContext deploymentContext, IOperationContext operationContext, IScriptRunner scriptRunner, IScriptParser scriptParser) : base(deploymentContext, fileSystemOperations, operationContext)
        {
            this.scriptPath = scriptPath;
            this.scriptRunner = scriptRunner;
            this.scriptParser = scriptParser;
        }

        protected override async Task ExecuteCore()
        {
            var newWorkDirectory = Path.GetDirectoryName(scriptPath);
            var script = scriptParser.Parse(File.ReadAllText(scriptPath));

            using (new DirectorySwitch(newWorkDirectory))
            {
                await scriptRunner.Run(script);
            }
        }

        private class DirectorySwitch : IDisposable
        {
            private readonly string oldDirectory;

            public DirectorySwitch(string directory)
            {
                Log.Debug($"Switching to {directory}");
                oldDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = directory;
            }

            public void Dispose()
            {
                Log.Debug($"Returning to {oldDirectory}");
                Environment.CurrentDirectory = oldDirectory;
            }
        }
    }
}