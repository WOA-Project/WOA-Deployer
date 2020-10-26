using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace Deployer.Tasks
{
    public class ZipExtractor : IZipExtractor
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipExtractor(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public async Task ExtractFirstChildToFolder(Stream stream, string destination,
            IOperationProgress progressObserver = null)
        {
            await ExtractCore(stream, destination,
                zipArchive =>
                {
                    var baseEntry = FirstChild(zipArchive.Entries);
                    var contents = zipArchive.Entries.Where(x => x.Key.StartsWith(baseEntry) && !x.IsDirectory);
                    return contents;
                }, FirstChild, progressObserver);
        }

        public async Task ExtractToFolder(Stream stream, string destination, IOperationProgress progressObserver = null)
        {
            await ExtractCore(stream, destination,
                zipArchive =>
                {
                    var contents = zipArchive.Entries.Where(x => !x.IsDirectory);
                    return contents;
                }, progressObserver: progressObserver);
        }

        public async Task ExtractRelativeFolder(Stream stream, string relativeZipPath, string destination,
            IOperationProgress progressObserver = null)
        {
            await ExtractCore(stream, destination,
                zipArchive =>
                {
                    relativeZipPath = relativeZipPath.EndsWith("/") ? relativeZipPath : relativeZipPath + "/" ;
                    var contents = zipArchive.Entries.Where(x => x.Key.StartsWith(relativeZipPath) && !x.IsDirectory);
                    return contents;
                }, progressObserver: progressObserver, baseEntry: entries => entries.First(x => x.IsDirectory && x.Key.Equals(relativeZipPath)).Key);
        }

        public async Task ExtractRelativeFolder(Stream stream, Func<IEnumerable<ZipArchiveEntry>, string> getSourceFolder, string destination, IOperationProgress progressObserver = null)
        {
            await ExtractCore(stream, destination,
                zipArchive =>
                {
                    var baseEntry = getSourceFolder(zipArchive.Entries);
                    var contents = zipArchive.Entries.Where(x => x.Key.StartsWith(baseEntry) && !x.IsDirectory);
                    return contents;
                }, getSourceFolder, progressObserver);
        }

        private async Task ExtractCore(Stream stream, string destination,
            Func<ZipArchive, IEnumerable<ZipArchiveEntry>> selectEntries,
            Func<IEnumerable<ZipArchiveEntry>, string> baseEntry = null, IOperationProgress progressObserver = null)
        {
            var archive = ZipArchive.Open(stream);
            var entries = selectEntries(archive);
            await ExtractContents(entries.ToList(), destination, baseEntry?.Invoke(archive.Entries), progressObserver);
        }

        private async Task ExtractContents(ICollection<ZipArchiveEntry> entries, string destination,
            string baseEntry = null, IOperationProgress progressObserver = null)
        {
            var total = entries.Count;
            var copied = 0;

            foreach (var entry in entries)
            {
                var filePath = entry.Key.Substring(baseEntry?.Length ?? 0);

                var destFile = Path.Combine(destination, filePath.Replace("/", "\\"));
                var dir = Path.GetDirectoryName(destFile);
                if (!fileSystemOperations.DirectoryExists(dir))
                {
                    fileSystemOperations.CreateDirectory(dir);
                }

                using (var destStream = File.Open(destFile, FileMode.OpenOrCreate))
                using (var stream = entry.OpenEntryStream())
                {
                    await stream.CopyToAsync(destStream);
                    copied++;
                    progressObserver?.Percentage.OnNext(copied / (double)total);
                }
            }

            progressObserver?.Percentage.OnNext(double.NaN);
        }

        private string FirstChild(IEnumerable<ZipArchiveEntry> zipArchiveEntries)
        {
            var split = zipArchiveEntries.Select(x => x.Key.Split('/'))
                .OrderBy(s => s.Length)
                .First()
                .First();

            return split + "/";
        }        
    }
}