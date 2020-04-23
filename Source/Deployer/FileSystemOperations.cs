using System.IO;
using System.Threading.Tasks;
using Deployer.Utils;
using Serilog;

namespace Deployer
{
    public class FileSystemOperations : IFileSystemOperations
    {
        public Task Copy(string source, string destination)
        {
            Log.Debug("Copying file {Source} to {Destination}", source, destination);

            return FileUtils.Copy(source, destination);
        }

        public Task CopyDirectory(string source, string destination, string fileSearchPattern = null)
        {
            Log.Verbose("Copying directory {Source} to {Destination}", source, destination);
            return FileUtils.CopyDirectory(source, destination, fileSearchPattern);
        }

        public Task DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!DirectoryExists(directoryPath))
            {
                CreateDirectory(directoryPath);
            }
        }

        public Task DeleteFile(string filePath)
        {
            File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}