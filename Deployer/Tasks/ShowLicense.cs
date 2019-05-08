using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Deployer.Execution;

namespace Deployer.Tasks
{
    [TaskDescription("License from {0}")]
    public class ShowLicense : IDeploymentTask
    {
        private readonly string path;
        private readonly IPrompt dialog;

        public ShowLicense(string path, IPrompt dialog)
        {
            this.path = path;
            this.dialog = dialog;
        }

        public async Task Execute()
        {
            var msg = File.ReadAllText(path);
            var result = await dialog.PickOptions(msg, new List<Option>()
            {
                new Option("Accept", OptionValue.OK),
                new Option("Decline", OptionValue.Cancel),
            });

            if (result.OptionValue == OptionValue.Cancel)
            {
                throw new LicenseAgreementDeclinedException("The license has been declined. Deployment canceled.");
            }
        }
    }
}