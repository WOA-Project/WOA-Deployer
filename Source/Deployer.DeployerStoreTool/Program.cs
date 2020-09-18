using System.Diagnostics;
using System.IO;
using System.Xml;
using Deployer.Core;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace Deployer.DeployerStoreTool
{
    class Program
    {
        private const string OutputXml = "output.xml";

        static void Main(string[] args)
        {
            var ds = DefaultStore.GetDeployerStore();

            var serializer = new ConfigurationContainer()
                .Type<Device>().EnableReferences(x => x.Id)
                .UseOptimizedNamespaces()
                .Create();

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
