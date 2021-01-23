using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Filesystem;
using Zafiro.Core.FileSystem;

namespace Deployer.Functions
{
    public class EnsureValidPartitionLayout : DeployerFunction
    {
        private readonly IFileSystem fileSystem;
        private IEnumerable<string> names;

        public EnsureValidPartitionLayout(IFileSystem fileSystem, IFileSystemOperations fileSystemOperations, IOperationContext operationContext) : base(fileSystemOperations, operationContext)
        {
            this.fileSystem = fileSystem;
        }

        public async Task Execute(int diskNumber)
        {
            var disk = await fileSystem.GetDisk(diskNumber);
            var parts = await disk.GetPartitions();
            names = parts.Select(x => x.GptName);
            var shouldBlock =
                Contains("EFIESP") && Contains("SYSTEM") && Contains("MSR") && Contains("Data") &&
                (!Contains("MainOS") || !Contains("MMOS")) || Contains("OSPool") || !Contains("EFIESP");

            if (shouldBlock)
            {
                throw new ApplicationException("The partition layout is not supported.");
            }
        }

        private bool Contains(string name)
        {
            return names.Contains(name, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
