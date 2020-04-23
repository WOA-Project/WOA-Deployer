using System;
using System.Threading.Tasks;
using Deployer.Core.FileSystem;
using Deployer.Core.Services;
using Deployer.Core.Utils;
using Serilog;

namespace Deployer.Core
{
    public class BootCreator : IBootCreator
    {
        private readonly Func<string, IBcdInvoker> bcdInvokerFactory;

        public BootCreator(Func<string, IBcdInvoker> bcdInvokerFactory)
        {
            this.bcdInvokerFactory = bcdInvokerFactory;
        }

        public async Task MakeBootable(string systemRoot, string windowsPath)
        {
            Log.Verbose("Making Windows installation bootable...");

            var bcdInvoker = bcdInvokerFactory(systemRoot.CombineRelativeBcdPath());

            await ProcessMixin.RunProcess(WindowsCommandLineUtils.BcdBoot, $@"{windowsPath} /f UEFI /s {systemRoot} /l en-us");
            await bcdInvoker.Invoke("/set {default} testsigning on");
            await bcdInvoker.Invoke("/set {default} nointegritychecks on");
        }
    }
}