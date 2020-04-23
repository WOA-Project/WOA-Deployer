using System;

namespace Deployer.Core.Scripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiresAttribute : Attribute
    {
        public Dependency Dependency { get; }

        public RequiresAttribute(Dependency dependency)
        {
            Dependency = dependency;
        }
    }
}