using System;
using System.Collections.Generic;

namespace Deployer.Core
{
    public class RequirementException : Exception
    {
        public IEnumerable<string> Requirements { get; }

        public RequirementException(IEnumerable<string> requirements) : base(string.Join(",", requirements))
        {
            Requirements = requirements;
        }
    }
}