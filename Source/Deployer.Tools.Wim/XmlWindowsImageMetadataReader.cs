using System.IO;

namespace Deployer.Tools.Wim
{
    public class XmlWindowsImageMetadataReader : WindowsImageMetadataReaderBase
    {
        protected override Stream GetXmlMetadataStream(Stream wim)
        {
            return wim;
        }
    }
}