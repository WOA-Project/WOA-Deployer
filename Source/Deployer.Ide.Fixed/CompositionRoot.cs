using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using Deployer.Core;
using Deployer.Core.Compiler;
using Deployer.Core.Requirements;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services;
using Deployer.Net4x;
using Deployer.NetFx;
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
using IntrospectionExtensions = System.Reflection.IntrospectionExtensions;
using MarkdownService = Deployer.Wpf.MarkdownService;
using Popup = Zafiro.UI.Popup;

namespace Deployer.Ide
{
    public static class CompositionRoot
    {
        public static DependencyInjectionContainer CreateContainer()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c =>
            {
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
                c.ExportFactory(() => new WoaDeployerWpf()).As<WoaDeployerBase>().Lifestyle.Singleton();
                
                foreach (var taskType in TaskTypes)
                {
                    c.ExportFactory((Func<Type, object> locator) => new Function(taskType, locator))
                        .As<IFunction>()
                        .As<IFunctionDeclaration>();
                }
            });

            return container;
        }

        public static IEnumerable<Type> TaskTypes
        {
            get
            {
                var taskTypes = from a in new[] { typeof(IAnchor).Assembly }
                    from type in a.ExportedTypes
                    where IntrospectionExtensions.GetTypeInfo(type).ImplementedInterfaces.Contains(typeof(IDeployerFunction))
                    where !type.IsAbstract 
                    select type;
                return taskTypes;
            }
        }
    }
}