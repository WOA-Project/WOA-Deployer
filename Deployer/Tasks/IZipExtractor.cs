using System;
using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public interface IZipExtractor
    {
        Task ExtractFirstChildToFolder(Stream stream, string destination, IObserver<double> progressObserver = null);
        Task ExtractToFolder(Stream stream, string folderPath, IObserver<double> progressObserver = null);
        Task ExtractRelativeFolder(Stream stream, string relativeZipPath, string destination, IObserver<double> progressObserver = null);
    }
}