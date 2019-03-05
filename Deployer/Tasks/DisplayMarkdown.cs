using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("Displaying Markdown document from {0}")]
    public class DisplayMarkdown : IDeploymentTask
    {
        private readonly string path;
        private readonly IMarkdownDialog dialog;

        public DisplayMarkdown(string path, IMarkdownDialog dialog)
        {
            this.path = path;
            this.dialog = dialog;
        }

        public async Task Execute()
        {
            var msg = File.ReadAllText(path);
            await dialog.PickOptions(msg, new List<Option>()
            {
                new Option("Close", DialogValue.OK),
            });
        }
    }
}