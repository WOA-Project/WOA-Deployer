using System;

namespace Deployer.Tools.ImageFlashing
{
    public class FlashException : Exception
    {
        public FlashException(string msg) : base(msg)
        {
        }
    }
}