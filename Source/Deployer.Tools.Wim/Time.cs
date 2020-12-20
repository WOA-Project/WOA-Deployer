using System.Xml.Serialization;

namespace Deployer.Tools.Wim
{
    [XmlRoot(ElementName = "CREATIONTIME")]
        public class Time
        {
            [XmlElement(ElementName = "HIGHPART")] public string HighPart { get; set; }

            [XmlElement(ElementName = "LOWPART")] public string LowPart { get; set; }
        }
}