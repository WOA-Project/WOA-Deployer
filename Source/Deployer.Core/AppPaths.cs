using System.IO;

namespace Deployer.Core
{
    public static class AppPaths
    {
        public const string Feed = "Feed";
        public static readonly string Metadata = Path.Combine(Feed, "Metadata");
        public static readonly string LogDump = "LogDump";
    }
}