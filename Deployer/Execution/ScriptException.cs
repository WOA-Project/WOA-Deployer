using System;

namespace Deployer.Execution
{
    internal class ScriptException : Exception
    {
        public ScriptException(string msg, Exception innerException) : base(msg, innerException)
        {            
        }
    }
}