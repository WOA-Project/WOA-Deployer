using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zafiro.Core;

namespace Deployer.Core.Registrations
{
    public class DeviceRepository: IDeviceRepository
    {
        private readonly IDownloader downloader;

        public DeviceRepository(IDownloader downloader)
        {
            this.downloader = downloader;
        }

        public async Task<IEnumerable<Device>> GetAll()
        {
            using (var file = new StreamReader(await downloader.GetStream("https://raw.githubusercontent.com/WOA-Project/Deployment-Feed/master/Devices.json")))
            {
                var str = await file.ReadToEndAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Device>>(str);
            }
        }
    }
}