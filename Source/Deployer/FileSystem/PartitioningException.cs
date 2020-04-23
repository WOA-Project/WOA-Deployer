using System;

namespace Deployer.FileSystem
{
    public class PartitioningException : ApplicationException
    {
        public PartitioningException(string message) : base(message)
        {
        }
    }
}