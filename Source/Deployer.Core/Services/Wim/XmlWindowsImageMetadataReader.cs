using System.IO;

namespace Deployer.Core.Services.Wim
{
    public class XmlWindowsImageMetadataReader : WindowsImageMetadataReaderBase
    {
        protected override Stream GetXmlMetadataStream(Stream wim)
        {
            return wim;
        }
    }
}