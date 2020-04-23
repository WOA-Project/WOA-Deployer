using System;

namespace Deployer.Core.Exceptions
{
    public class DeploymentException : Exception
    {
        public DeploymentException(string msg) : base(msg)
        {            
        }
    }
}