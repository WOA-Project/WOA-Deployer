using System.IO;

namespace Deployer.Filesystem
{
    public class PathExtensions
    {
        public static string GetRootPath(char driveLetter)
        {
            return $"{driveLetter}:{Path.DirectorySeparatorChar}";
        }
    }
}