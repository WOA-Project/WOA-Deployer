using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    [TaskDescription("Displaying Markdown document from {0}")]
    public class DisplayMarkdown : DeploymentTask
    {
        private readonly string path;
        private readonly IDialog dialog;

        public DisplayMarkdown(string path, IDialog dialog, IDeploymentContext deploymentContext) : base(deploymentContext)
        {
            this.path = path;
            this.dialog = dialog;
        }

        protected override async Task ExecuteCore()
        {
            var msg = File.ReadAllText(path);
            await dialog.PickOptions(msg, new List<Option>()
            {
                new Option("Close", OptionValue.OK),
            });
        }
    }
}