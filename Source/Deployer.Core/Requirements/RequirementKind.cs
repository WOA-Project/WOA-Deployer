using System;
using System.Linq;
using Optional.Collections;

namespace Deployer.Core.Requirements
{
    public class RequirementKind
    {
        public static readonly RequirementKind Disk = new RequirementKind();
        public static RequirementKind WimFile =new RequirementKind();

        public static Optional.Option<RequirementKind> Parse(string str)
        {
            return OptionCollectionExtensions.FirstOrNone(typeof(RequirementKind).GetFields()
                    .Where(m => m.FieldType == typeof(RequirementKind)),
                m => m.Name.Equals(str,
                    StringComparison.InvariantCultureIgnoreCase)).Map(f => (RequirementKind) f.GetValue(null));
        }
    }
}