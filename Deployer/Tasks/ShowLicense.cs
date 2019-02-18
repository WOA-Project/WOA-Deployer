using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    public class ShowLicense : IDeploymentTask
    {
        private readonly string path;
        private readonly IMarkdownDialog dialog;

        public ShowLicense(string path, IMarkdownDialog dialog)
        {
            this.path = path;
            this.dialog = dialog;
        }

        public async Task Execute()
        {
            var result = await dialog.PickOptions(File.ReadAllText(path), new List<Option>()
            {
                new Option("Accept", DialogValue.OK),
                new Option("Decline", DialogValue.Cancel),
            });

            if (result.DialogValue == DialogValue.Cancel)
            {
                throw new LicenseAgreementDeclinedException("The license has been declined. Deployment canceled.");
            }
        }
    }
}