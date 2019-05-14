using System.IO;

namespace Deployer
{
    public static class AppPaths
    {
        public const string ArtifactDownload = "Downloaded";
        public static readonly string Metadata = Path.Combine(ArtifactDownload, "Metadata");
    }
}