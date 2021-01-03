using System;
using Deployer.Core;
using Deployer.Core.Compiler;
using Deployer.Core.Requirements;
using Deployer.Core.Services;
using Deployer.Functions;
using Deployer.Net4x;
using Deployer.Wpf;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Parsing;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.UI;
using Zafiro.UI.Wpf;

namespace Deployer.Ide
{
    public class Composition
    {
        private readonly DependencyInjectionContainer container;

        public Composition()
        {
            container = CreateContainer();
        }

        public MainViewModel Root => container.Locate<MainViewModel>();

        public static DependencyInjectionContainer CreateContainer()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c =>
            {
                c.Export<IdeDeployerCompiler>().As<IIdeDeployerCompiler>().Lifestyle.Singleton();
                c.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
                c.Export<Preprocessor>().As<IPreprocessor>().Lifestyle.Singleton();
                c.Export<Parser>().As<IParser>().Lifestyle.Singleton();
                c.Export<Binder>().As<IBinder>().Lifestyle.Singleton();
                c.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
                c.Export<Popup>().As<IPopup>().Lifestyle.Singleton();
                c.Export<MarkdownService>().As<IMarkdownService>().Lifestyle.Singleton();
                c.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
                c.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
                c.ExportFactory<string, IFileSystemOperations, IDownloader, IZafiroFile>((path, fo, dl) => new DesktopZafiroFile(new Uri(path), fo, dl));
                c.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>().Lifestyle.Singleton();
                c.ConfigureMediator();
                c.ExportFactory(() => new WoaDeployerWpf(new[] { typeof(Anchor).Assembly })).As<IWoaDeployer>().Lifestyle.Singleton();
                c.ExportFactory((IWoaDeployer d) => d.OperationProgress).As<IOperationProgress>().Lifestyle
                    .Singleton();
            });

            return container;
        }
    }
}