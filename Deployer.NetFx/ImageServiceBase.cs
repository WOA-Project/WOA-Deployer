using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Deployer.Exceptions;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Utils;
using Serilog;
using Zafiro.Core;

namespace Deployer.NetFx
{
    public abstract class ImageServiceBase : IWindowsImageService
    {
        private readonly IFileSystemOperations fileSystemOperations;

        protected ImageServiceBase(IFileSystemOperations fileSystemOperations)
        {
            this.fileSystemOperations = fileSystemOperations;
        }

        public abstract Task ApplyImage(IPartition target, string imagePath, int imageIndex = 1, bool useCompact = false,
            IOperationProgress progressObserver = null, CancellationToken token = default(CancellationToken));

        protected void EnsureValidParameters(IPartition applyPath, string imagePath, int imageIndex)
        {
            if (applyPath == null)
            {
                throw new ArgumentNullException(nameof(applyPath));
            }

            if (applyPath.Root == null)
            {
                throw new ArgumentException("The volume to apply the image is invalid");
            }

            if (imagePath == null)
            {
                throw new ArgumentNullException(nameof(imagePath));
            }

            EnsureValidImage(imagePath, imageIndex);
        }

        private void EnsureValidImage(string imagePath, int imageIndex)
        {
            Log.Verbose("Checking image at {Path}, with index {Index}", imagePath, imagePath);

            if (!fileSystemOperations.FileExists(imagePath))
            {
                throw new FileNotFoundException($"Image not found: {imagePath}. Please, verify that the file exists and it's accessible.");
            }

            Log.Verbose("Image file at '{ImagePath}' exists", imagePath);                    
        }

        public async Task<IList<string>> InjectDrivers(string path, string windowsRootPath)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var outputSubject = new Subject<string>();

            var subscription = outputSubject.Subscribe(Log.Verbose);

            var args = new[]
            {
                "/Add-Driver",
                $"/Image:{windowsRootPath}",
                $@"/Driver:""{path}""",
                IsUniqueFile(path) ? "" : "/Recurse",
            };

            var processResults = await ProcessMixin.RunProcess(WindowsCommandLineUtils.Dism, args.Join(" "), outputObserver: outputSubject, errorObserver: outputSubject);
            subscription.Dispose();
            
            if (processResults.ExitCode != 0)
            {
                throw new DeploymentException(
                    $"There has been a problem during deployment: DISM exited with code {processResults.ExitCode}. Output: {processResults.StandardOutput.Join()}");
            }

            return StringExtensions.ExtractFileNames(string.Concat(processResults.StandardOutput)).ToList();
        }

        private bool IsUniqueFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var hasInfExt = Path.GetExtension(path).Equals(".inf", StringComparison.InvariantCultureIgnoreCase);
            return hasInfExt && fileSystemOperations.FileExists(path);
        }


        public async Task RemoveDriver(string path, string windowsRootPath)
        {
            var outputSubject = new Subject<string>();
            var subscription = outputSubject.Subscribe(Log.Verbose);
            var processResults = await ProcessMixin.RunProcess(WindowsCommandLineUtils.Dism, $@"/Remove-Driver /Image:{windowsRootPath} /Driver:""{path}""", outputObserver: outputSubject, errorObserver: outputSubject);
            subscription.Dispose();
            
            if (processResults.ExitCode != 0)
            {
                throw new DeploymentException(
                    $"There has been a problem during removal: DISM exited with code {processResults}.");
            }
        }

        public abstract Task CaptureImage(IPartition source, string destination,
            IOperationProgress progressObserver = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}