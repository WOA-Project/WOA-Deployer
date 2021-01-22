using System;
using System.Linq;
using Optional;
using Optional.Collections;

namespace Deployer.Core.Requirements
{
    public class RequirementDefinition
    {
        public static readonly RequirementDefinition Disk = new RequirementDefinition();
        public static RequirementDefinition WimFile = new RequirementDefinition();

        public static Option<RequirementDefinition> Parse(string str)
        {
            var split = str.Split(':');
            if (split[0] == "Number")
            {
                return new NumberRequirementDefinition(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3])).Some<RequirementDefinition>();
            }
            
            return OptionCollectionExtensions.FirstOrNone(typeof(RequirementDefinition).GetFields()
                    .Where(m => m.FieldType == typeof(RequirementDefinition)),
                m => m.Name.Equals(str,
                    StringComparison.InvariantCultureIgnoreCase)).Map(f => (RequirementDefinition) f.GetValue(null));
        }
    }

    public class DiskRequirementDefinition : RequirementDefinition
    {
    }

    public class WimFileRequirementDefinition : RequirementDefinition
    {
    }

    public class NumberRequirementDefinition : RequirementDefinition
    {
        public double Min { get; }
        public double Max { get; }
        public double DefaultValue { get; set; }

        public NumberRequirementDefinition(double min, double defaultValue, double max)
        {
            Min = min;
            DefaultValue = defaultValue;
            Max = max;
        }
    }
}