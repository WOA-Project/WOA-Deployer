using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.DeploymentLibrary;
using Deployer.Core.Scripting.Core;
using Deployer.Filesystem;

namespace Deployer.Lumia
{
    public class LumiaContextualizer : IContextualizer
    {
        private readonly IFileSystem fileSystem;

        public LumiaContextualizer(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool CanContextualize(Device device)
        {
            var identifiers = new[] {"cityman", "talkman"};

            return identifiers.Any(x => device.Code.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) != -1);
        }

        public async Task Setup(IDictionary<string, object> variables)
        {
            var disk = await Detection.GetDisk(fileSystem);
            if (disk != null)
            {
                variables[Requirement.Disk] = disk.Number;
            }
        }
    }
}