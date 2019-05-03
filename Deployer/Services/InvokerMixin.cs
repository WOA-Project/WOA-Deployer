using System;

namespace Deployer.Services
{
    public static class InvokerMixin
    {
        public static void SafeCreate(this IBcdInvoker invoker, Guid guid, string args)
        {
            if (invoker.Invoke($"/enum {{{guid}}}").Contains("description"))
            {
                return;
            }

            invoker.Invoke($"/create {{{guid}}} {args}");
        }
    }
}