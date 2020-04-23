using System;
using System.Threading.Tasks;

namespace Deployer.Services
{
    public static class InvokerMixin
    {
        public static async Task SafeCreate(this IBcdInvoker invoker, Guid guid, string args)
        {
            var output = await invoker.Invoke($"/enum {{{guid}}}");
            var alreadyExists = output.Contains("{") && output.Contains("}");

            if (alreadyExists)
            {
                return;
            }

            await invoker.Invoke($"/create {{{guid}}} {args}");
        }
    }
}