using System;

namespace Deployer.Tasks
{
    public class LicenseAgreementDeclinedException : Exception
    {
        public LicenseAgreementDeclinedException(string message) : base(message)
        {           
        }
    }
}