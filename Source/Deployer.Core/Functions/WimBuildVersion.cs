using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Core.Scripting;
using Deployer.Core.Scripting.Core;
using Deployer.Core.Services.Wim;
using Optional.Collections;
using Zafiro.Core.FileSystem;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Functions
{
    public class WimBuildVersion : DeployerFunction
    {
        private readonly IWindowsImageMetadataReader imageMetadataReader;

        public WimBuildVersion(IFileSystemOperations fileSystemOperations, IOperationContext operationContext, IWindowsImageMetadataReader imageMetadataReader) : base(fileSystemOperations, operationContext)
        {
            this.imageMetadataReader = imageMetadataReader;
        }

        public Task<string> Execute(string path, int index)
        {
            using (Stream stream = FileSystemOperations.OpenForRead(path))
            {
                var image = imageMetadataReader.Load(stream);
                var version = image.MapRight(x => LinqEnumerableExtensions
                        .FirstOrNone(x.Images, z => z.Index == index)
                        .Match(y => y.Build, () => ""))
                    .Handle(x => "");

                return Task.FromResult(version);
            }
        }
    }
}
