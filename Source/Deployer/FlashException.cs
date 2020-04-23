using System;

namespace Deployer
{
    public class FlashException : Exception
    {
        public FlashException(string msg) : base(msg)
        {
        }
    }
}