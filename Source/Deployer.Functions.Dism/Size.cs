using System.Threading.Tasks;
using ByteSizeLib;
using Deployer.Core.Scripting;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class Size : DeployerFunction
    {

        public Size(IFileSystemOperations fileSystemOperations,
            IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
        }

        public Task<int> Execute(string one, string two)
        {
            var oneSize = ByteSize.Parse(one);
            var twoSize = ByteSize.Parse(two);

            return Task.FromResult(oneSize.CompareTo(twoSize));
        }
    }
}