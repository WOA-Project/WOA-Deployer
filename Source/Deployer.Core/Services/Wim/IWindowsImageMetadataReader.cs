using System.IO;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Core.Services.Wim
{
    public interface IWindowsImageMetadataReader
    {
        Either<ErrorList, XmlWindowsImageMetadata> Load(Stream stream);
    }
}