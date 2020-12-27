using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Deployer.Core.DeploymentLibrary.Utils.LazyTask;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Zafiro.Core;

namespace Deployer.Core.DeploymentLibrary
{
    public class XmlDeploymentLibrary : IDeploymentLibrary
    {
        private readonly Uri uri;
        private readonly IDownloader downloader;
        private readonly IExtendedXmlSerializer serializer;
        

        public XmlDeploymentLibrary(Uri uri, IDownloader downloader)
        {
            this.uri = uri;
            this.downloader = downloader;
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
            using (var stream = await downloader.GetStream(uri.ToString()))
            {
                var deserialize = serializer.Deserialize(XmlReader.Create((Stream) stream));
                var store = (DeployerStore)deserialize;
                return store;
            }
        }
    }

    public interface IDeploymentLibrary
    {
        Task<List<DeviceDto>> Devices();
        Task<List<DeploymentDto>> Deployments();
    }

    public class DeviceDto
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
    }

    public class DeploymentDto
    {
        public IEnumerable<int> Devices { get; set; }
        public string ScriptPath { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
    }
}