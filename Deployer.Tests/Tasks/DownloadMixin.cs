using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Deployer.Tests.Tasks
{
    public static class DownloadMixin
    {
        public static async Task Download(string url, string destination)
        {
            if (Directory.Exists(destination))
            {
                return;
            }

            var client = new HttpClient();
            using (var stream = await client.GetStreamAsync(url))
            {
                var zipFile = new ZipArchive(stream, ZipArchiveMode.Read);
                zipFile.ExtractToDirectory(destination);
            }
        }
    }
}