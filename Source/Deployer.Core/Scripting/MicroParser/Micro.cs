using System;
using System.Collections.Generic;
using System.Linq;

namespace Deployer.Core.Scripting.MicroParser
{
    public class Micro
    {
        private readonly Dictionary<string, object> values;
        public Assignment[] Assignments { get; }

        public Micro(Assignment[] assignments)
        {
            Assignments = assignments;
            values = Assignments.ToDictionary(x => x.Identifier, x => x.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        public object this[string key]
        {
            get
            {
                if (!values.TryGetValue(key, out var value))
                {
                    return null;
                }

                return value;
            }
        }
    }
}