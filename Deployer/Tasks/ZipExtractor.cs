using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public class ZipExtractor : IZipExtractor
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipExtractor(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task ExtractFirstChildToFolder(Stream stream, string destination)
        {
            var archive = await Observable.Start(() => new ZipArchive(stream, ZipArchiveMode.Read));
            using (var zipArchive = archive)
            {
                var baseEntry = FirstChild(zipArchive.Entries);
                var contents = zipArchive.Entries.Where(x => x.FullName.StartsWith(baseEntry) && !x.FullName.EndsWith("/"));
                await ExtractContents(contents, destination, baseEntry);
            }
        }

        private string FirstChild(IEnumerable<ZipArchiveEntry> zipArchiveEntries)
        {
            return zipArchiveEntries.First(x => x.FullName.EndsWith("/")).FullName;
        }

        public async Task ExtractToFolder(Stream stream, string destination)
        {
            var archive = await Observable.Start(() => new ZipArchive(stream, ZipArchiveMode.Read));
            using (var zipArchive = archive)
            {
                var contents = zipArchive.Entries.Where(x => !x.FullName.EndsWith("/"));
                await ExtractContents(contents, destination);
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