using System;

namespace Deployer.Core.FileSystem
{
    public class PartitioningException : ApplicationException
    {
        public PartitioningException(string message) : base(message)
        {
        }
    }
}