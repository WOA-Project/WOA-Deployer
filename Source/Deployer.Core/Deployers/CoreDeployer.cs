using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers.Errors.Compiler;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Requirements;
using Iridio.Binding.Model;
using Iridio.Runtime;
using Iridio.Runtime.ReturnValues;
using NLog;
using Serilog;
using Zafiro.Core;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Deployers
{
    public class CoreDeployer
    {
        private readonly IFileSystemOperations fso;
        private readonly IScriptRunner scriptRunner;
        private readonly IRequirementsManager reqsManager;
        private readonly IOperationProgress operationProgress;
        private readonly ISubject<string> internalMessages = new Subject<string>();
        public IDeployerCompiler Compiler { get; }

        public CoreDeployer(IDeployerCompiler compiler, IFileSystemOperations fso, IScriptRunner scriptRunner, IRequirementsManager reqsManager, IOperationProgress operationProgress)
        {
            this.fso = fso;
            this.scriptRunner = scriptRunner;
            this.reqsManager = reqsManager;
            this.operationProgress = operationProgress;
            Compiler = compiler;
            Messages = scriptRunner.Messages.Merge(internalMessages);
        }

        public IObservable<string> Messages { get; }

        public async Task<Either<DeployerError, Success>> Run(string path)
        {
            operationProgress.Send(new Unknown());
            
            using (new DirectorySwitch(fso, Path.GetDirectoryName(path)))
            {
                var fileName = Path.GetFileName(path);
                var satisfyResult = await reqsManager.Satisfy(fileName);
                var mapLeft = await satisfyResult
                    .MapLeft(error => (DeployerError)new RequirementsFailed(new UnableToSatisfyRequirements(error)))
                    .MapRight(toInject => Compile(fileName, toInject).MapLeft(errors => (DeployerError) new CompilationFailed(new UnableToCompile(errors) )))
                    .MapRight(async c =>
                    {
                        var task = await scriptRunner.Run(c, new Dictionary<string, object>());
                        return task.MapLeft(delegate(RuntimeErrors errors)
                        {
                            var deployerError = (DeployerError) new ExecutionFailed(errors);
                            Log.Error($"The deployment has failed: {deployerError}");
                            return deployerError;
                        });
                    })
                    .RightTask();
                operationProgress.Send(new Done());
                internalMessages.OnNext("");
                return mapLeft;
            }

            
        }

        private Either<Iridio.Common.Errors, Script> Compile(string path, IEnumerable<FulfilledRequirement> toInject)
        {
            var assignments = GetAssignments(toInject);
            var compilation = Compiler.Compile(path, assignments);
            return compilation;
        }

        private static IEnumerable<Assignment> GetAssignments(IEnumerable<FulfilledRequirement> toInject)
        {
            return toInject.Select(req => new Assignment(req.Key, req.Value));
        }
    }
}