using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;

namespace Deployer.Lumia
{
    public class LumiaContextualizer : IContextualizer
    {
        private readonly IFileSystem fileSystem;
        private readonly Device[] lumias = new[] { Device.CitymanDs, Device.CitymanSs, Device.TalkmanDs, Device.TalkmanSs, };

        public LumiaContextualizer(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool CanContextualize(Device device)
        {
            return lumias.Contains(device);
        }

        public async Task Setup(IDictionary<string, object> variables)
        {
            var disk = await Detection.GetDisk(fileSystem);
            if (disk == null)
            {
                throw new InvalidOperationException("Cannot detect the phone disk. Is the phone connected in Mass Storage Mode?");
            }

            variables[Requirement.Disk] = disk.Number;
        }
    }
}