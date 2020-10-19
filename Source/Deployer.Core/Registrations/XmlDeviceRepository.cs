using System;
using System.Threading.Tasks;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Zafiro.Core;

namespace Deployer.Core.Registrations
{
    public class XmlDeviceRepository: IDevRepo
    {
        private readonly Uri uri;
        private readonly IDownloader downloader;
        private readonly IExtendedXmlSerializer serializer;

        public XmlDeviceRepository(Uri uri, IDownloader downloader)
        {
            this.uri = uri;
            this.downloader = downloader;
            this.serializer = new ConfigurationContainer()
                .Type<Device>().EnableReferences(x => x.Id)
                .Create();
        }

        public async Task<DeployerStore> Get()
        {
            using (var stream = await downloader.GetStream(uri.ToString()))
            {
                var deserialize = serializer.Deserialize(XmlReader.Create(stream));
                var store = (DeployerStore) deserialize;
                return store;
            }
        }
    }
}