using System;

namespace Deployer.Core.Exceptions
{
    public class PhoneDiskNotFoundException : Exception
    {
        public PhoneDiskNotFoundException(string message)  : base(message)
        {
            
        }
    }
}