using System;

namespace Deployer.Core.Exceptions
{
    internal class PathNotFoundException : Exception
    {
        public PathNotFoundException(string msg) : base(msg)
        {
        }
    }
}