using System.IO;
using System.Threading.Tasks;
using Serilog;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using Zafiro.Core.FileSystem;

namespace Deployer.Core
{
    public class LogCollector : ILogCollector
    {
        private readonly IFileSystemOperations fileSystemOperations;
        private string winPath;

        public LogCollector(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        private void DeleteExistingDump()
        {
            if (fileSystemOperations.DirectoryExists(AppPaths.LogDump))
            {
                fileSystemOperations.DeleteDirectory(AppPaths.LogDump);
            }
        }

        private void ZipDumpedDirectories(string savePath)
        {
            var zipArchive = ZipArchive.Create();
            if (!fileSystemOperations.DirectoryExists(AppPaths.LogDump))
            {
                throw new NothingToSaveException();
            }

            zipArchive.AddAllFromDirectory(AppPaths.LogDump);
            using (var stream = File.OpenWrite(savePath))
            {
                zipArchive.SaveTo(stream);
            }
        }

        private async Task DumpDirectories()
        {
            var copyTuples = new[]
            {
                ("Panther", "*"),
                ("Minidump", "*"),
                ("CrashDump", "*"),
                ("", "MEMORY*.dmp"),
                ("INF", "*log*"),
                ("Logs", "*"),
                (Path.Combine("System32", "Winevt", "Logs"), "*")
            };
            
            foreach (var (dir, fileSearchPattern) in copyTuples)
            {
                var sourceDir = Path.Combine(winPath, dir);
                var destination = Path.Combine(AppPaths.LogDump, dir);
                try
                {
                    await fileSystemOperations.CopyDirectory(sourceDir, destination, fileSearchPattern);
                }
                catch (DirectoryNotFoundException e)
                {
                    Log.Error(e, "Could not dump directory {Directory}", dir);
                }
            }
        }
    }
}