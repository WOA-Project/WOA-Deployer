using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SharpCompress.Archives.Zip;

namespace Deployer.Tasks
{
    public interface IZipExtractor
    {
        Task ExtractFirstChildToFolder(Stream stream, string destination, IOperationProgress progressObserver = null);
        Task ExtractToFolder(Stream stream, string folderPath, IOperationProgress progressObserver = null);
        Task ExtractRelativeFolder(Stream stream, string relativeZipPath, string destination, IOperationProgress progressObserver = null);
        Task ExtractRelativeFolder(Stream stream, Func<IEnumerable<ZipArchiveEntry>, ZipArchiveEntry> getSourceFolder, string destination, IOperationProgress progressObserver = null);
    }
}