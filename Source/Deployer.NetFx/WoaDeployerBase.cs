using System;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Requirements;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Deployer.NetFx;
using Grace.DependencyInjection;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Parsing;
using Iridio.Runtime;
using Iridio.Runtime.ReturnValues;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Net4x;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Net4x
{
    public abstract class WoaDeployer : IWoaDeployer
    {
        private readonly CoreDeployer deployer;

        protected WoaDeployer()
        {
            deployer = GetDeployer();
        }

        public IOperationProgress OperationProgress { get; } = new OperationProgress();

        public IOperationContext OperationContext { get; } = new OperationContext();

        public IObservable<string> Messages => deployer.Messages;

        public Task<Either<DeployerError, Success>> Run(string scriptPath)
        {
            return deployer.Run(scriptPath);
        }

        private CoreDeployer GetDeployer()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(block =>
            {
                FunctionDependencies.Configure(block);
                block.Export<FileSystemOperations>().As<IFileSystemOperations>().Lifestyle.Singleton();
                block.Export<Preprocessor>().As<IPreprocessor>().Lifestyle.Singleton();
                block.Export<Parser>().As<IParser>().Lifestyle.Singleton();
                block.Export<Binder>().As<IBinder>().Lifestyle.Singleton();
                block.Export<DeployerCompiler>().As<IDeployerCompiler>().Lifestyle.Singleton();
                block.Export<Downloader>().As<IDownloader>().Lifestyle.Singleton();
                block.ExportFactory<string, IFileSystemOperations, IDownloader, IZafiroFile>((path, fo, dl) =>
                    new ZafiroFile(new Uri(path), fo, dl));
                block.Export<IridioRequirementsAnalyzer>().As<IRequirementsAnalyzer>().Lifestyle.Singleton();
                block.Export<ScriptRunner>().As<IScriptRunner>().Lifestyle.Singleton();
                block.ExportFactory(() => OperationProgress).As<IOperationProgress>().Lifestyle.Singleton();
                block.ExportFactory(() => OperationContext).As<IOperationContext>().Lifestyle.Singleton();
                block.Export<ShellOpen>().As<IShellOpen>().Lifestyle.Singleton();
                block.Export<FileSystem>().As<IFileSystem>().Lifestyle.Singleton();
                ExportFunctions(block);
                ExportSpecificDependencies(block);
            });

            return container.Locate<CoreDeployer>();
        }

        private static void ExportFunctions(IExportRegistrationBlock block)
        {
            foreach (var taskType in Function.Types)
                block.ExportFactory((Func<Type, object> locator) => new Function(taskType, locator))
                    .As<IFunction>()
                    .As<IFunctionDeclaration>()
                    .Lifestyle.Singleton();
        }

        protected virtual void ExportSpecificDependencies(IExportRegistrationBlock exportRegistrationBlock)
        {
        }
    }
}