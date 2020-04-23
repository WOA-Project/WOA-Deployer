using System;

namespace Deployer.Core
{
    public interface IInstanceBuilder
    {
        object Create(Type type, params object[] parameters);
    }
}