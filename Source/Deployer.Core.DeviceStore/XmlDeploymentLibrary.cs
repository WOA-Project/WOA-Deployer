using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Deployer.Core.DeploymentLibrary.Utils.LazyTask;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.DeploymentLibrary
{
    public class XmlDeploymentLibrary : IDeploymentLibrary
    {
        private readonly string path;
        private readonly IFileSystemOperations ops;
        private readonly IExtendedXmlSerializer serializer;
        

        public XmlDeploymentLibrary(string path, IFileSystemOperations ops)
        {
            this.path = path;
            this.ops = ops;
            this.serializer = new ConfigurationContainer()
                .Type<Device>().EnableReferences(x => x.Id)
                .Create();
        }
        
        public async Task<List<DeviceDto>> Devices()
        {
            var deployerStore = await DeployerStore();
            return deployerStore.Devices.Select(d => new DeviceDto()
            {
                Id = d.Id,
                Icon =d.Icon,
                Code = d.Code,
                Description = d.Description,
                FriendlyName = d.FriendlyName,
                Name = d.Name,
                Variant = d.Variant,
            }).ToList();
        }

        public async Task<List<DeploymentDto>> Deployments()
        {
            var deployerStore = await DeployerStore();
            return deployerStore.Deployments.Select(d => new DeploymentDto()
            {
                Icon = d.Icon,
                Description = d.Description,
                Devices = d.Devices.Select(x => x.Id),
                ScriptPath = d.ScriptPath,
                Title = d.Title,
            }).ToList();
        }

        private async LazyTask<DeployerStore> DeployerStore()
        {
            using (var stream = ops.OpenForRead(path))
            {
                var deserialize = serializer.Deserialize(XmlReader.Create((Stream) stream));
                var store = (DeployerStore)deserialize;
                return store;
            }
        }
    }
}