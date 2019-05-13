using System;
using System.Collections.Generic;
using System.Linq;

namespace Deployer.UI
{
    public class ViewMappings : Dictionary<object, Type>
    {
        public ViewMappings(params (object, Type)[] mappings) : base(mappings.ToDictionary(x => x.Item1, x => x.Item2))
        {
        }
    }
}