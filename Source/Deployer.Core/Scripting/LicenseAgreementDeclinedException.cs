using System;

namespace Deployer.Core.Scripting
{
    public class LicenseAgreementDeclinedException : Exception
    {
        public LicenseAgreementDeclinedException(string message) : base(message)
        {           
        }
    }
}