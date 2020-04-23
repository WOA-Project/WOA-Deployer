using System.Collections.Generic;

namespace Deployer.Core.Services.Wim
{
    public class XmlWindowsImageMetadata
    {
        public IList<DiskImageMetadata> Images { get; set; }
    }
}