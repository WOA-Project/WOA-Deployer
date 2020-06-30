using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using Zafiro.Core.FileSystem;

namespace Deployer.Core.Scripting
{
    public class ZipExtractor : IZipExtractor
    {
        private readonly IFileSystemOperations fileSystemOperations;

        public ZipExtractor(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public Task Extract(Stream stream, string destination,
            IOperationProgress progressObserver = null)
        {
            return ExtractCore(stream, destination, GetContents, GetTopFolderEntry, progressObserver);
        }

        public Task ExtractRoot(Stream stream, string destination,
            IOperationProgress progressObserver = null)
        {
            return ExtractCore(stream, destination, archive => archive.Entries.Where(x => !x.IsDirectory), null, progressObserver);
        }

        private IEnumerable<ZipArchiveEntry> GetContents(ZipArchive zipArchive)
        {
            var topFolderEntry = GetTopFolderEntry(zipArchive.Entries);
            IEnumerable<ZipArchiveEntry> contents = zipArchive.Entries.Where(x => !x.IsDirectory);

            if (topFolderEntry != null)
            {
                contents = contents.Where(x => x.Key.StartsWith(topFolderEntry.Key));
            }

            return contents;
        }

        private async Task ExtractCore(Stream stream, string destination,
            Func<ZipArchive, IEnumerable<ZipArchiveEntry>> selectEntries,
            Func<IEnumerable<ZipArchiveEntry>, ZipArchiveEntry> sourceFolderSelector = null, IOperationProgress progressObserver = null)
        {
            using (var archive = ZipArchive.Open(stream))
            {
                var entries = selectEntries(archive);
                await ExtractContents(entries.ToList(), destination, sourceFolderSelector?.Invoke(archive.Entries), progressObserver);
            }
        }

        private async Task ExtractContents(ICollection<ZipArchiveEntry> entries, string destination,
            IEntry baseEntry = null, IOperationProgress progressObserver = null)
        {
            var total = entries.Count;
            var copied = 0;

            foreach (var entry in entries)
            {
                var filePath = entry.Key.Substring(baseEntry?.Key.Length ?? 0);

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

        private ZipArchiveEntry GetTopFolderEntry(IEnumerable<ZipArchiveEntry> zipArchiveEntries)
        {
            var entries = zipArchiveEntries.Where(IsTopLevelEntry).ToList();

            if (entries.Count == 1)
            {
                return entries.First();
            }

            return null;
        }

        private static bool IsTopLevelEntry(ZipArchiveEntry entry)
        {
            return entry.IsDirectory && entry.Key.Split('/').Length == 2;
        }
    }
}