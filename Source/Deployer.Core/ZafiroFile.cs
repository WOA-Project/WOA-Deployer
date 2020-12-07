using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class ZafiroFile : IZafiroFile
    {
        private readonly Uri uri;
        private readonly IFileSystemOperations fileSystemOperations;
        private readonly IDownloader downloader;

        public ZafiroFile(
            Uri uri,
            IFileSystemOperations fileSystemOperations,
            IDownloader downloader)
        {
            this.uri = uri;
            this.fileSystemOperations = fileSystemOperations;
            this.downloader = downloader;
        }

        public Task<Stream> OpenForRead() => uri.IsFile ? Task.FromResult(fileSystemOperations.OpenForRead(uri.LocalPath)) : downloader.GetStream(uri.ToString());

        public async Task<Stream> OpenForWrite()
        {
            if (!uri.IsFile)
                throw new NotSupportedException();
            await fileSystemOperations.Truncate(uri.LocalPath);
            return fileSystemOperations.OpenForWrite(uri.LocalPath);
        }

        public string Name => ((IEnumerable<string>)uri.Segments).Last<string>();

        public Uri Source => uri;
    }
}