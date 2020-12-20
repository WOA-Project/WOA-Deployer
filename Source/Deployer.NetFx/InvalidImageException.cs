using System;

namespace Deployer.Net4x
{
    internal class InvalidImageException : Exception
    {
        public InvalidImageException(string msg) : base(msg)
        {            
        }
    }
}