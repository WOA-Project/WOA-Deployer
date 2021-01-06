using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private Lazy<Task<DeployerStore>> lazyStore;


        public XmlDeploymentLibrary(string path, IFileSystemOperations ops)
        {
            this.path = path;
            this.ops = ops;
            this.serializer = new ConfigurationContainer()
                .Type<Device>().EnableReferences(x => x.Id)
                .Create();
            lazyStore = GetDeployerStore();
        }
        
        public async Task<List<DeviceDto>> Devices()
        {
            var deployerStore = await lazyStore.Value;
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
            var deployerStore = await lazyStore.Value;
            return deployerStore.Deployments.Select(d => new DeploymentDto()
            {
                Id = d.Id,
                Icon = d.Icon,
                Description = d.Description,
                Devices = d.Devices.Select(x => x.Id).ToList(),
                ScriptPath = d.ScriptPath,
                Title = d.Title,
            }).ToList();
        }

        private Lazy<Task<DeployerStore>> GetDeployerStore()
        {
            using (var stream = ops.OpenForRead(path))
            {
                var deserialize = serializer.Deserialize(XmlReader.Create(stream));
                var store = (DeployerStore) deserialize;
                return new Lazy<Task<DeployerStore>>(() => Task.FromResult(store));
            }
        }

        public static IExtendedXmlSerializer CreateSerializer() =>
            new ConfigurationContainer()
                .Type<Device>().EnableReferences(x => x.Id)
                .Type<Deployment>().Member(x => x.Id).Attribute()
                .UseOptimizedNamespaces()
                .Create();
    }
}