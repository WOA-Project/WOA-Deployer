using System;

namespace Deployer.FileSystem
{
    public class FileSystemException : Exception
    {
        public FileSystemException(string msg) : base(msg)
        {
        }
    }
}