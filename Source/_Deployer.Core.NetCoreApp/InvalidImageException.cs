using System;

namespace Deployer.Core.NetCoreApp
{
    internal class InvalidImageException : Exception
    {
        public InvalidImageException(string msg) : base(msg)
        {            
        }
    }
}