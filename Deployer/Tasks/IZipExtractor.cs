using System.IO;
using System.Threading.Tasks;

namespace Deployer.Tasks
{
    public interface IZipExtractor
    {
        Task ExtractFirstChildToFolder(Stream stream, string folderPath);
        Task ExtractToFolder(Stream stream, string folderPath);
        Task ExtractRelativeFolder(Stream stream, string relativeZipPath, string destination);
    }
}