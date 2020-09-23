using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Core.FileSystem;
using Iridio.Binding;
using Iridio.Binding.Model;
using Iridio.Common;
using Iridio.Parsing;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Tests
{
    public class DeployerScriptRunner
    {
        private readonly IRequirementSatisfier satisfier;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IRequirementsAnalyzer requirementsAnalyzer;
        private Parser parser;
        private Binder binder;

        public DeployerScriptRunner(IRequirementSatisfier satisfier, IEnumerable<IFunction> functions, IFileSystemOperations fileSystemOperations, IRequirementsAnalyzer requirementsAnalyzer)
        {
            this.satisfier = satisfier;
            this.fileSystemOperations = fileSystemOperations;
            this.requirementsAnalyzer = requirementsAnalyzer;
            parser = new Parser();
            binder = new Binder(new BindingContext(functions));
        }

        public async Task<Either<Errors, CompilationUnit>> Run(string path, IEnumerable<Requirement> providedRequirements)
        {
            var preprocessor = new Preprocessor(fileSystemOperations);
            var requirements = requirementsAnalyzer.GetRequirements(fileSystemOperations.ReadAllText(path));
            
            
            var input = preprocessor.Process(path);
            var parsed = parser.Parse(input);
            return parsed
                .MapLeft(error => new Errors(new Error(ErrorKind.UnableToParse, error.Message)))
                .MapRight(script => binder.Bind(script));
        }

        
    }

    public interface IRequirementsAnalyzer
    {
        IEnumerable<Requirement> GetRequirements(string content);
    }

    public interface IRequirementsSatisfier
    {
        IEnumerable<Requirement> Satisfy(IEnumerable<MissingRequirement> content);
    }

    public class MissingRequirement
    {
        public string Name { get; set; }
        public string Kind { get; set; }
    }

    public class IridioRequirementsAnalyzer : IRequirementsAnalyzer
    {
        public IEnumerable<Requirement> GetRequirements(string content)
        {
            var pattern = @"(?i)\s*//\s*Requires\s+([A-Za-z_]+[\dA-Za-z_])\s+""(.+)""\s+as\s+""(.+)""";
            var matches = Regex.Matches(content, pattern);
            return matches.Cast<Match>()
                .Select(m => new Requirement(m.Groups[2].Value, m.Groups[1].Value, m.Groups[3].Value));
        }
    }
}