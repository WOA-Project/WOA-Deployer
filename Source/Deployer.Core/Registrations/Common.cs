using System;
using System.Net.Http;
using Deployer.Core.DevOpsBuildClient;
using Deployer.Core.Scripting;
using Grace.DependencyInjection;
using Octokit;
using Octokit.Internal;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Registrations
{
    public class Common : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock block)
        {
            block.Export<ZipExtractor>().As<IZipExtractor>();
            block.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
            block.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
            block.ExportFactory(() => new HttpClient { Timeout = TimeSpan.FromMinutes(30) }).Lifestyle.Singleton();
            block.ExportFactory(() => new GitHubClient(new ProductHeaderValue("WOADeployer"), new InMemoryCredentialStore(new Credentials("3aeb9225109bd0a77c690196ebb5759ecde5469e", AuthenticationType.Bearer)))).As<IGitHubClient>().Lifestyle.Singleton();
            block.ExportFactory(() => AzureDevOpsBuildClient.Create(new Uri("https://dev.azure.com"))).As<IAzureDevOpsBuildClient>().Lifestyle.Singleton();
        }
    }
}