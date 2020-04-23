using System;

namespace Deployer.Core
{
    public class UndeterminedDeviceException : ApplicationException
    {

        public UndeterminedDeviceException()
        {
        }

        public UndeterminedDeviceException(string msg) : base(msg)
        {
        }
    }
}