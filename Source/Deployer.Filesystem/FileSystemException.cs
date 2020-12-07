using System;

namespace Deployer.Filesystem
{
    public class FileSystemException : Exception
    {
        public FileSystemException(string msg) : base(msg)
        {
        }
    }
}