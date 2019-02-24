using System;

namespace Deployer.NetFx
{
    internal class InvalidImageException : Exception
    {
        public InvalidImageException(string msg) : base(msg)
        {            
        }
    }
}