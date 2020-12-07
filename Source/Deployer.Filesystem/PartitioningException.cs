using System;

namespace Deployer.Filesystem
{
    public class PartitioningException : ApplicationException
    {
        public PartitioningException(string message) : base(message)
        {
        }
    }
}