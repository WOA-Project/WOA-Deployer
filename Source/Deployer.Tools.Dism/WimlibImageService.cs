using System;
using System.Threading;
using System.Threading.Tasks;
using Deployer.Filesystem;
using ManagedWimLib;
using Zafiro.Core;
using Zafiro.Core.FileSystem;

namespace Deployer.Tools.Dism
{
    public class WimlibImageService : ImageServiceBase
    {
        public WimlibImageService(IFileSystemOperations fileSystemOperations) : base(fileSystemOperations)
        {
        }

        public override Task ApplyImage(IPartition target, string imagePath, int imageIndex = 1,
            bool useCompact = false, IOperationProgress progressObserver = null,
            CancellationToken token = default(CancellationToken))
        {
            return ApplyImage(target.Root, imagePath, imageIndex, useCompact, progressObserver, token);
        }

        public override Task CaptureImage(IPartition source, string destination,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override Task ApplyImage(string targetDriveRoot, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        private static CallbackStatus UpdatedStatusCallback(ProgressMsg msg, object info, object progctx,
            IOperationProgress progressObserver)
        {
            if (info is ProgressInfo_Extract m)
            {
                ulong percentComplete = 0;

                switch (msg)
                {
                    case ProgressMsg.EXTRACT_FILE_STRUCTURE:

                        if (0 < m.EndFileCount) percentComplete = m.CurrentFileCount * 10 / m.EndFileCount;

                        break;
                    case ProgressMsg.EXTRACT_STREAMS:

                        if (0 < m.TotalBytes) percentComplete = 10 + m.CompletedBytes * 80 / m.TotalBytes;

                        break;
                    case ProgressMsg.EXTRACT_METADATA:

                        if (0 < m.EndFileCount) percentComplete = 90 + m.CurrentFileCount * 10 / m.EndFileCount;

                        break;
                }

                progressObserver.Send(new Percentage((double) percentComplete / 100));
            }


            return CallbackStatus.CONTINUE;
        }
    }
}