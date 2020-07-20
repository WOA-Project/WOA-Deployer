using System.Threading.Tasks;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Zafiro.Core;

namespace Deployer.Core.Registrations
{
    public class DeviceRepository: IDeviceRepository
    {
        private readonly IDownloader downloader;
        private readonly IExtendedXmlSerializer serializer;

        public DeviceRepository(IDownloader downloader)
        {
            this.downloader = downloader;
            this.serializer = new ConfigurationContainer()
                .EnableReferences()
                .Create();
        }

        public async Task<DeployerStore> Get()
        {
            using (var stream = await downloader.GetStream("https://raw.githubusercontent.com/WOA-Project/Deployment-Feed/master/Deployments.xml"))
            {
                var deserialize = serializer.Deserialize(XmlReader.Create(stream));
                var store = (DeployerStore) deserialize;
                return store;
            }
        }
    }
}