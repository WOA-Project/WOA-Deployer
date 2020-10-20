using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Requirements;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Runtime;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Deployers
{
    public class BrandNewDeployer : BrandNewDeployerBase
    {
        private readonly IScriptRunner scriptRunner;
        private readonly IRequirementsManager reqsManager;
        public IDeployerCompiler Compiler { get; }

        public BrandNewDeployer(IDeployerCompiler compiler, IScriptRunner scriptRunner, IRequirementsManager reqsManager)
        {
            this.scriptRunner = scriptRunner;
            this.reqsManager = reqsManager;
            Compiler = compiler;
        }

        public async Task<Either<DeployError, Success>> Run(string path)
        {
            var satisfyResult = await reqsManager.Satisfy(path);
            var mapLeft = await satisfyResult
                .MapLeft(error => (DeployError)new RequirementsError(error))
                .MapRight(toInject => Compile(path, toInject).MapLeft(errors => (DeployError) new CompileError(errors)))
                .MapRight(async c =>
                {
                    var task = await scriptRunner.Run(c, new Dictionary<string, object>());
                    return task.MapLeft(errors => (DeployError) new ExecutionError(errors));
                })
                .RightTask();
            return mapLeft;
        }

        private Either<Errors, CompilationUnit> Compile(string path, IEnumerable<FulfilledRequirement> toInject)
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