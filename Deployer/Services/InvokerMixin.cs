using System;

namespace Deployer.Services
{
    public static class InvokerMixin
    {
        public static void SafeCreate(this IBcdInvoker invoker, Guid guid, string args)
        {
            var output = invoker.Invoke($"/enum {{{guid}}}");
            var alreadyExists = output.Contains("{") && output.Contains("}");

            if (alreadyExists)
            {
                return;
            }

            invoker.Invoke($"/create {{{guid}}} {args}");
        }
    }
}