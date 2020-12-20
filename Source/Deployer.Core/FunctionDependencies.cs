using System;
using System.Net.Http;
using Deployer.Core.Scripting;
using Deployer.Core.Services;
using Deployer.Tools.AzureDevOps;
using Deployer.Tools.Dism;
using Deployer.Tools.Wim;
using Grace.DependencyInjection;
using Octokit;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class FunctionDependencies
    {
        public static void Configure(IExportRegistrationBlock block)
        {
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
            block.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
            block.ExportFactory(() => new HttpClient { Timeout = TimeSpan.FromMinutes(30) }).Lifestyle.Singleton();
            block.ExportFactory(() => new GitHubClient(new ProductHeaderValue("WOADeployer"))).As<IGitHubClient>()
                .Lifestyle.Singleton();
            block.ExportFactory(() => AzureDevOpsBuildClient.Create(new Uri("https://dev.azure.com")))
                .As<IAzureDevOpsBuildClient>().Lifestyle.Singleton();
            block.ExportFactory((IDownloader downloader) => XmlDeviceRepository(downloader)).As<IDevRepo>().Lifestyle
                .Singleton();
            block.Export<BootCreator>().As<IBootCreator>().Lifestyle.Singleton();
            block.ExportFactory((string store) => new BcdInvoker(store)).As<IBcdInvoker>();
            block.Export<ImageFlasher>().As<IImageFlasher>().Lifestyle.Singleton();
            block.Export<DismImageService>().As<IWindowsImageService>().Lifestyle.Singleton();
            block.Export<WindowsImageMetadataReader>().As<IWindowsImageMetadataReader>().Lifestyle.Singleton();
        }

        private static XmlDeviceRepository XmlDeviceRepository(IDownloader downloader)
        {
            var definition = "https://raw.githubusercontent.com/WOA-Project/Deployment-Feed/master/Deployments.xml";
            return new XmlDeviceRepository(new Uri(definition), downloader);
        }
    }
}