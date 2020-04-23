using System;

namespace Deployer.Core.Exceptions
{
    internal class ExtractionException : Exception
    {
        public ExtractionException(string msg) : base(msg)
        {            
        }
    }
}