using System;

namespace Deployer.Core.Exceptions
{
    public class NotEnoughSpaceException : Exception
    {
        public NotEnoughSpaceException(string msg) : base(msg)
        {
        }
    }
}