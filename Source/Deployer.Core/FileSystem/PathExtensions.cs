using System.IO;

namespace Deployer.Core.FileSystem
{
    public class PathExtensions
    {
        public static string GetRootPath(char driveLetter)
        {
            return $"{driveLetter}:{Path.DirectorySeparatorChar}";
        }
    }
}