using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Requirements;
using Iridio.Common;
using Iridio.Runtime;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core
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

        protected async Task<Either<Errors, Success>> Run(string path)
        {
            var toInject= await reqsManager.Satisfy(path);
            var assignments = GetAssignments(toInject);
            var script = Compiler.Compile(path, assignments);
            var runResult = script.MapRight(cu => scriptRunner.Run(cu, new Dictionary<string, object>()));
            var executionResult = await runResult.RightTask();
            return executionResult;
        }

        private static IEnumerable<Assignment> GetAssignments(IEnumerable<FulfilledRequirement> toInject)
        {
            return toInject.Select(req => new Assignment(req.Key, req.Value));
        }
    }
}