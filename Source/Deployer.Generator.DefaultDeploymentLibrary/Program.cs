using System.Diagnostics;
using System.IO;
using System.Xml;
using Deployer.Core;
using Deployer.Core.DeploymentLibrary;
using ExtendedXmlSerializer;

namespace Deployer.Generator.DefaultDeploymentLibrary
{
    class Program
    {
        private const string OutputXml = "output.xml";

        static void Main(string[] args)
        {
            var ds = DefaultStore.GetDeployerStore();

            var serializer = XmlDeploymentLibrary.CreateSerializer();

            var serialized = serializer.Serialize(new XmlWriterSettings() {Indent = true}, ds);

            File.WriteAllText(OutputXml, serialized);

            var psi = new ProcessStartInfo(OutputXml)
            {
                UseShellExecute = true,
            };

            Process.Start(psi);
        }
    }
}
