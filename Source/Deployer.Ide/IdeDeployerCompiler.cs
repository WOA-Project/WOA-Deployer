using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Compiler;
using Deployer.Core.Deployers.Errors;
using Deployer.Core.Deployers.Errors.Compiler;
using Deployer.Core.Requirements;
using Deployer.Core.Requirements.Disk;
using Iridio.Binding.Model;
using MediatR;
using Zafiro.Core;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Ide
{
    internal class IdeDeployerCompiler : IIdeDeployerCompiler
    {
        private readonly IDeployerCompiler compiler;
        private readonly IRequirementsAnalyzer requirementsAnalyzer;
        private readonly ISender mediator;
        private readonly IFileSystemOperations fileSystemOperations;

        public IdeDeployerCompiler(IDeployerCompiler compiler, IRequirementsAnalyzer requirementsAnalyzer, ISender mediator, IFileSystemOperations fileSystemOperations)
        {
            this.compiler = compiler;
            this.requirementsAnalyzer = requirementsAnalyzer;
            this.mediator = mediator;
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task<Either<DeployerCompilerError, Script>> Compile(string path)
        {
            var fileContents = fileSystemOperations.ReadAllText(path);
            var requirements = requirementsAnalyzer.GetRequirements(fileContents);
            var ret = requirements.MapRight(async req =>
                {
                    var satisfiedReqs = await SatisfyRequirements(req);
                    var assignmentsToInject = satisfiedReqs.Select(TurnIntoAssignments);
                    return compiler.Compile(path, assignmentsToInject)
                        .MapLeft(x => (DeployerCompilerError) new UnableToCompile(x));
                })
                .MapLeft(errors => (DeployerCompilerError) new UnableToSatisfyRequirements(errors));

            var rightTask = await ret.RightTask();
            return rightTask;
        }

        private async Task<IEnumerable<FulfilledRequirement>> SatisfyRequirements(IEnumerable<MissingRequirement> requirements)
        {
            var responses = await requirements.Select(ToRequirementRequest)
                .AsyncSelect(async re =>
                {
                    var send = await mediator.Send(re);
                    return (RequirementResponse)send;
                });

            var selectMany = responses.SelectMany(x => x);

            return selectMany;
        }

        private static RequirementRequest ToRequirementRequest(MissingRequirement r)
        {
            if (r.Kind == RequirementKind.WimFile)
            {
                return new WimFileRequest {Index = 0, Path = "", Key = r.Key};
            }

            if (r.Kind == RequirementKind.Disk)
            {
                return new DiskRequest {Index = 0, Key = r.Key};
            }

            throw new ArgumentOutOfRangeException();
        }

        private Assignment TurnIntoAssignments(FulfilledRequirement responses)
        {
            return new Assignment(responses.Key, "#placeholder#");
        }
    }
}