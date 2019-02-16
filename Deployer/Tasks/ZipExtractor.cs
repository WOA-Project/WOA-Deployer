using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Utils;

namespace Deployer.Tasks
{
    public class ZipExtractor : IZipExtractor
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipExtractor(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task ExtractFirstChildToFolder(Stream stream, string folderPath)
        {
            var tempDir = FileUtils.GetTempDirectoryName();
            await Extract(stream, folderPath, tempDir);
            await MoveFirstChildToDestination(tempDir, folderPath);
        }

        public async Task ExtractToFolder(Stream stream, string folderPath)
        {
            var tempDir = FileUtils.GetTempDirectoryName();
            await Extract(stream, folderPath, tempDir);
            await MoveToDestination(tempDir, folderPath);
        }

        private async Task MoveToDestination(string source, string destination)
        {
            await fileSystemOperations.CopyDirectory(source, destination);
            await fileSystemOperations.DeleteDirectory(source);
        }

        private async Task MoveFirstChildToDestination(string source, string destination)
        {
            var folderName = Path.GetFileName(destination);
            var firstChild = Path.Combine(source, folderName);

            await fileSystemOperations.CopyDirectory(firstChild, destination);
            await fileSystemOperations.DeleteDirectory(source);
        }

        private async Task Extract(Stream stream, string folderPath, string temp)
        {
            await Observable.Start(() =>
            {
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read, false))
                {
                    zip.ExtractToDirectory(temp);
                }
            });

            if (fileSystemOperations.DirectoryExists(folderPath))
            {
                await fileSystemOperations.DeleteDirectory(folderPath);
            }
        }

        public async Task ExtractRelativeFolder(Stream stream, string relativeZipPath, string destination)
        {
            var archive = await Observable.Start(() => new ZipArchive(stream, ZipArchiveMode.Read));
            using (var zipArchive = archive)
            {
                var baseEntry = RootPath(relativeZipPath);
                var contents = zipArchive.Entries.Where(x => x.FullName.StartsWith(baseEntry) && !x.FullName.EndsWith("/"));
                await ExtractContents(contents, destination, baseEntry);
            }
        }

        private static string RootPath(string relativeZipPath)
        {
            relativeZipPath = relativeZipPath.Replace('\\', '/');
            return relativeZipPath.EndsWith("//") ? relativeZipPath : relativeZipPath + "/";
        }

        private async Task ExtractContents(IEnumerable<ZipArchiveEntry> entries, string destination,
            string baseEntryPath = "")
        {
            foreach (var entry in entries)
            {
                var filePath = entry.FullName.Substring(baseEntryPath.Length);

                var destFile = Path.Combine(destination, filePath.Replace("/", "\\"));
                var dir = Path.GetDirectoryName(destFile);
                if (!fileSystemOperations.DirectoryExists(dir))
                {
                    fileSystemOperations.CreateDirectory(dir);
                }

                using (var destStream = File.Open(destFile, FileMode.OpenOrCreate))
                using (var stream = entry.Open())
                {
                    await stream.CopyToAsync(destStream);
                }
            }
        }
    }
}