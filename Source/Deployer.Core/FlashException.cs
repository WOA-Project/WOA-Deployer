using System;

namespace Deployer.Core
{
    public class FlashException : Exception
    {
        public FlashException(string msg) : base(msg)
        {
        }
    }
}