using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    [TaskDescription("License from {0}")]
    public class ShowLicense : DeploymentTask
    {
        private readonly string path;
        private readonly IDialog dialog;

        public ShowLicense(string path, IDialog dialog, IDeploymentContext deploymentContext,
            IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(deploymentContext, fileSystemOperations, operationContext)
        {
            this.path = path;
            this.dialog = dialog;
        }

        protected override async Task ExecuteCore()
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