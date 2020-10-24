using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Deployer.Core;
using Deployer.Core.Compiler;
using Deployer.Core.Scripting.Core;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Parsing;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Wpf;
using Zafiro.Wpf.Services;
using Binder = Iridio.Binding.Binder;

namespace Editor.Wpf
{
    public class Composition
    {
        private readonly DependencyInjectionContainer container;

        public Composition()
        {
            container = CompositionRoot.CreateContainer();
        }

        public MainViewModel Root
        {
            get
            {
                return container.Locate<MainViewModel>();
            }
        }
    }

    public class CompositionRoot
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
                c.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
                c.Export<OpenFilePicker>().As<IOpenFilePicker>().Lifestyle.Singleton();
                c.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();

                c.ExportFactory<string, IFileSystemOperations, IDownloader, IZafiroFile>((path, fo, downloader) => new DesktopZafiroFile(new Uri(path), fo, downloader));

                foreach (var taskType in TaskTypes)
                {
                    c.ExportFactory((Func<Type, object> locator) => new Function(taskType, locator)).As<IFunction>();
                }
            });
            return container;
        }

        public void Configure(IExportRegistrationBlock block)
        {
            foreach (var taskType in TaskTypes)
            {
                block.ExportFactory((Func<Type, object> locator) => new Function(taskType, locator)).As<IFunction>();
            }
        }

        public static IEnumerable<Type> TaskTypes
        {
            get
            {
                var taskTypes = from a in new[] { typeof(IDeployerFunction).Assembly }
                    from type in a.ExportedTypes
                    where type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IDeployerFunction))
                    where !type.IsAbstract 
                    select type;
                return taskTypes;
            }
        }
    }
}