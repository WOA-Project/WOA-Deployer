using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers.Errors;
using Deployer.Core.Deployers.Errors.Compiler;
using Deployer.Core.Deployers.Errors.Deployer;
using Deployer.Core.Requirements;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Runtime;
using Iridio.Runtime.ReturnValues;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Deployers
{
    public class BrandNewDeployer : BrandNewDeployerBase
    {
        private readonly IFileSystemOperations fso;
        private readonly IScriptRunner scriptRunner;
        private readonly IRequirementsManager reqsManager;
        public IDeployerCompiler Compiler { get; }

        public BrandNewDeployer(IDeployerCompiler compiler, IFileSystemOperations fso, IScriptRunner scriptRunner, IRequirementsManager reqsManager)
        {
            this.fso = fso;
            this.scriptRunner = scriptRunner;
            this.reqsManager = reqsManager;
            Compiler = compiler;
            Messages = scriptRunner.Messages;
        }

        public IObservable<string> Messages { get; }

        public async Task<Either<DeployerError, Success>> Run(string path)
        {
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
                        return task.MapLeft(errors => (DeployerError) new ExecutionFailed(errors));
                    })
                    .RightTask();
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