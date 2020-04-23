using System;

namespace Deployer.Core.FileSystem
{
    public class FileSystemException : Exception
    {
        public FileSystemException(string msg) : base(msg)
        {
        }
    }
}