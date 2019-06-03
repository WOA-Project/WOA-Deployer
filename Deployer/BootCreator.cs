using System.IO;
using System.Threading.Tasks;
using Deployer.FileSystem;
using Deployer.Services;
using Deployer.Utils;
using Serilog;

namespace Deployer
{
    public class BootCreator : IBootCreator
    {
        private readonly IBcdInvokerFactory bcdInvokerFactory;

        public BootCreator(IBcdInvokerFactory bcdInvokerFactory)
        {
            this.bcdInvokerFactory = bcdInvokerFactory;
        }

        public async Task MakeBootable(IPartition systemPartition, IPartition windowsPartition)
        {
            Log.Verbose("Making Windows installation bootable...");

            var bcdInvoker = bcdInvokerFactory.Create(systemPartition.Root.CombineRelativeBcdPath());
            var windowsPath = Path.Combine(windowsPartition.Root, "Windows");

            await ProcessMixin.RunProcess(WindowsCommandLineUtils.BcdBoot, $@"{windowsPath} /f UEFI /s {systemPartition.Root} /l en-us");
            await bcdInvoker.Invoke("/set {default} testsigning on");
            await bcdInvoker.Invoke("/set {default} nointegritychecks on");

            await systemPartition.SetGptType(PartitionType.Esp);
        }              
    }
}