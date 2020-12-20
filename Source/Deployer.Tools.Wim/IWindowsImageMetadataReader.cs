using System.IO;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Tools.Wim
{
    public interface IWindowsImageMetadataReader
    {
        Either<ErrorList, XmlWindowsImageMetadata> Load(Stream stream);
    }
}