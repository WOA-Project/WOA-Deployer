using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Deployer.FileSystem.Gpt;

namespace Deployer
{
    public static class GptContextFactory
    {
        public static Task<GptContext> Create(uint diskId, FileAccess fileAccess,
            uint bytesPerSector = GptContext.DefaultBytesPerSector, uint chuckSize = GptContext.DefaultChunkSize)
        {
            return Observable
                .Defer(() => Observable.Return(new GptContext(diskId, fileAccess, bytesPerSector, chuckSize)))
                .RetryWithBackoffStrategy()
                .ToTask();
        }
    }
}