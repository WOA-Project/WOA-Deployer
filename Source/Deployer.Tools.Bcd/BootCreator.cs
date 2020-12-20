using System;
using System.IO;
using System.Threading.Tasks;
using Deployer.Tools.Common;
using Serilog;

namespace Deployer.Tools.Bcd
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

            var bcdInvoker = bcdInvokerFactory(CombineRelativeBcdPath(systemRoot));

            await ProcessMixin.RunProcess(WindowsCommandLineUtils.BcdBoot, $@"{windowsPath} /f UEFI /s {systemRoot} /l en-us");
            await bcdInvoker.Invoke("/set {default} testsigning on");
            await bcdInvoker.Invoke("/set {default} nointegritychecks on");
        }

        private static string CombineRelativeBcdPath(string root)
        {
            return Path.Combine(root, "EFI", "Microsoft", "Boot", "BCD");
        }
    }
}