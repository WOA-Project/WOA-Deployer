using System.Collections.Generic;
using System.Threading.Tasks;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Requirements
{
    public class RequirementsManager : IRequirementsManager 
    {
        private readonly IRequirementsAnalyzer requirementsAnalyzer;
        private readonly IRequirementSupplier supplier;
        private readonly IFileSystemOperations fileSystemOperations;

        public RequirementsManager(IFileSystemOperations fileSystemOperations, IRequirementsAnalyzer requirementsAnalyzer, IRequirementSupplier supplier)
        {
            this.fileSystemOperations = fileSystemOperations;
            this.requirementsAnalyzer = requirementsAnalyzer;
            this.supplier = supplier;
        }

        public async Task<IEnumerable<FulfilledRequirement>> Satisfy(string path)
        {
            var requirements = requirementsAnalyzer.GetRequirements(fileSystemOperations.ReadAllText(path));
            var satisfied = await supplier.Satisfy(requirements);
            return satisfied;
        }
    }
}