using System;

namespace Deployer.Tasks
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