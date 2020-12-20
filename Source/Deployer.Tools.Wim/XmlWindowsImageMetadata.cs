using System.Collections.Generic;

namespace Deployer.Tools.Wim
{
    public class XmlWindowsImageMetadata
    {
        public IList<DiskImageMetadata> Images { get; set; } = new List<DiskImageMetadata>();
    }
}