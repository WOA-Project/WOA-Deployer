using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers;
using Deployer.Core.Requirements;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Parsing;
using Iridio.Runtime;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;
using Binder = Iridio.Binding.Binder;

namespace Deployer.Core
{
    public class ZafiroFile : IZafiroFile
    {
        private readonly Uri uri;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;

        public ZafiroFile(
            Uri uri,
            IFileSystemOperations fileSystemOperations,
            IDownloader downloader)
        {
            this.uri = uri;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
        }

        public Task<Stream> OpenForRead() => uri.IsFile ? Task.FromResult(fileSystemOperations.OpenForRead(uri.LocalPath)) : downloader.GetStream(uri.ToString());

        public async Task<Stream> OpenForWrite()
        {
            if (!uri.IsFile)
                throw new NotSupportedException();
            await fileSystemOperations.Truncate(uri.LocalPath);
            return fileSystemOperations.OpenForWrite(uri.LocalPath);
        }

        public string Name => ((IEnumerable<string>)uri.Segments).Last<string>();

        public Uri Source => uri;
    }

    public class WoaDeployer
    {
        private readonly IRequirementsManager requirementsManager;
        private readonly BrandNewDeployer deployer;

        public WoaDeployer(IRequirementsManager requirementsManager)
        {
            this.requirementsManager = requirementsManager;
            deployer = GetDeployer();
        }

        private BrandNewDeployer GetDeployer()
        {
            var container = new DependencyInjectionContainer();
            container.Configure(c =>
            {
                c.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
                c.Export<Preprocessor>().As<IPreprocessor>().Lifestyle.Singleton();
                c.Export<Parser>().As<IParser>().Lifestyle.Singleton();
                c.Export<Binder>().As<IBinder>().Lifestyle.Singleton();
                c.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
                c.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
                c.ExportFactory<string, IFileSystemOperations, IDownloader, IZafiroFile>((path, fo, dl) => new ZafiroFile(new Uri(path), fo, dl));
                c.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>().Lifestyle.Singleton();
                c.Export<ScriptRunner>().As<IScriptRunner>().Lifestyle.Singleton();
                c.ExportFactory(() => requirementsManager).As<IRequirementsManager>();
                c.Export<OperationContext>().As<IOperationContext>().Lifestyle.Singleton();
                c.Export<OperationProgress>().As<IOperationProgress>().Lifestyle.Singleton();


                foreach (var taskType in TaskTypes)
                {
                    c.ExportFactory((Func<Type, object> locator) => new Function(taskType, locator))
                        .As<IFunction>()
                        .As<IFunctionDeclaration>();
                }
            });

            return container.Locate<BrandNewDeployer>();
        }

        private static IEnumerable<Type> TaskTypes
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

        private BrandNewDeployer ConfigureContainer()
        {
            throw new NotImplementedException();
        }

        public Task<Either<DeployError, Success>> Run(string s)
        {
            return deployer.Run(s);
        }
    }
}