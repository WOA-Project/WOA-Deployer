using System.IO;

namespace Deployer.Core.Services.Wim
{
    public interface IWindowsImageMetadataReader
    {
        XmlWindowsImageMetadata Load(Stream stream);
    }
}