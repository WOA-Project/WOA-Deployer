using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deployer.Core;
using Deployer.Filesystem;
using Deployer.Filesystem.Gpt;
using Deployer.Tools.Bcd;
using Registry;
using Serilog;

namespace Deployer.Functions.Legacy
{
    public class LumiaDualBootAssistant
    {
        private const string WindowsSystem32BootWinloadEfi = @"windows\system32\boot\winload.efi";
        private readonly IFileSystem fileSystem;
        private readonly Func<string, IBcdInvoker> invokerFactory;
        private IDisk disk;
        private readonly int diskNumber;
        private IPartition systemPartition;

        public LumiaDualBootAssistant(int diskNumber, Func<string, IBcdInvoker> invokerFactory, IFileSystem fileSystem)
        {
            this.diskNumber = diskNumber;
            this.invokerFactory = invokerFactory;
            this.fileSystem = fileSystem;
        }

        public async Task ToogleDualBoot(bool isEnabled, bool force = false)
        {
            var status = await GetDualBootStatus();

            if (!force && !status.CanDualBoot) 
                throw new InvalidOperationException("Cannot enable Dual Boot. Please, verify that Windows 10 on ARM (WOA) is installed completely and that it boots to the Desktop");

            if (status.IsEnabled != isEnabled)
            {
                if (isEnabled)
                    await EnableDualBoot();
                else
                    await DisableDualBoot();
            }
            else
            {
                var statusStr = isEnabled ? "Enabled" : "Disabled";
                throw new InvalidOperationException($"Dual Boot status operation not performed: Dual Boot was already {statusStr}");
            }
        }

        private async Task DisableDualBoot()
        {
            Log.Verbose("Disabling Dual Boot...");

            var systemPartition = await GetSystemPartition();

            await systemPartition.SetGptType(GptType.Esp);
            var invoker = await GetBcdInvoker();
            await invoker.Invoke($@"/set {{{BcdGuids.WinMobile}}} description ""Dummy, please ignore""");
            await invoker.Invoke($@"/set {{{BcdGuids.WinMobile}}} path ""dummy""");
            await invoker.Invoke($@"/default {{{BcdGuids.Woa}}}");
            Log.Verbose("Dual Boot disabled");
        }

        private async Task EnableDualBoot()
        {
            Log.Information("Enabling Dual Boot...");

            var systemPartition = await GetSystemPartition();
            await systemPartition.SetGptType(GptType.Basic);

            var invoker = await GetBcdInvoker();
            await invoker.Invoke($@"/set {{{BcdGuids.WinMobile}}} description ""Windows 10 Phone""");
            await invoker.Invoke($@"/set {{{BcdGuids.WinMobile}}} path ""\windows\system32\boot\winload.efi""");
            await invoker.Invoke($@"/default {{{BcdGuids.WinMobile}}}");
            await invoker.Invoke($@"/displayorder {{{BcdGuids.WinMobile}}} /addfirst");

            Log.Information("Dual Boot enabled");
        }

        private async Task<IBcdInvoker> GetBcdInvoker()
        {
            var systemPartition = await GetSystemPartition();
            var efiEspRoot = systemPartition.Root;
            var store = $"{efiEspRoot}\\EFI\\Microsoft\\Boot\\BCD";
            var invoker = invokerFactory(store);
            return invoker;
        }

        private async Task<IPartition> GetSystemPartition()
        {
            if (systemPartition is null)
            {
                var disk = await GetDeviceDisk();
                var partition = await disk.GetPartitionByName(PartitionName.System);
                systemPartition = partition;
            }

            return systemPartition;
        }

        private async Task<bool> IsWindowsPhonePresent()
        {
            var disk = await GetDeviceDisk();
            using (var context = await GptContextFactory.Create(disk.Number, FileAccess.Read))
            {
                return context.Get(PartitionName.MainOs) != null && context.Get(PartitionName.Data) != null;
            }
        }

        private async Task<IDisk> GetDeviceDisk()
        {
            if (disk is null)
            {
                var deviceDisk = await fileSystem.GetDisk(diskNumber);
                disk = deviceDisk;
            }

            return disk;
        }

        private async Task<DualBootStatus> GetDualBootStatus()
        {
            Log.Verbose("Getting Dual Boot Status...");

            var isWoaPresent = await IsWoAPresent();
            var isWPhonePresent = await IsWindowsPhonePresent();
            var isOobeFinished = await IsOobeFinished();
            var isWinPhoneEntryPresent = await IsWindowsPhoneBcdEntryPresent();

            var systemPartition = await GetSystemPartition();

            var isEnabled = systemPartition.GptType.Equals(GptType.Basic) &&
                            isWinPhoneEntryPresent;

            var isCapable = isWoaPresent && isWPhonePresent && isOobeFinished;
            var status = new DualBootStatus(isCapable, isEnabled);

            Log.Verbose("WOA Present: {Value}", isWoaPresent);
            Log.Verbose("Windows 10 Mobile Present: {Value}", isWPhonePresent);
            Log.Verbose("OOBE Finished: {Value}", isOobeFinished);

            Log.Verbose("Dual Boot Status retrieved");
            Log.Verbose("Dual Boot Status is {@Status}", status);

            return status;
        }

        private async Task<bool> IsWindowsPhoneBcdEntryPresent()
        {
            var invoker = await GetBcdInvoker();
            var result = await invoker.Invoke();

            var containsWinLoad = Contains(result, WindowsSystem32BootWinloadEfi,
                StringComparison.CurrentCultureIgnoreCase);
            var containsWinPhoneBcdGuid = Contains(result, BcdGuids.WinMobile.ToString(),
                StringComparison.InvariantCultureIgnoreCase);

            return containsWinLoad || containsWinPhoneBcdGuid;
        }

        private async Task<bool> IsWoAPresent()
        {
            var disk = await fileSystem.GetDisk(diskNumber);
            using (var context = await GptContextFactory.Create(disk.Number, FileAccess.Read))
            {
                return context.Get(PartitionName.Windows) != null && context.Get(PartitionName.System) != null;
            }
        }

        private async Task<bool> IsOobeFinished()
        {
            var winVolume = await GetWindowsPartition();

            if (winVolume == null) return false;

            var path = Path.Combine(winVolume.Root, "Windows", "System32", "Config", "System");
            if (!File.Exists(path)) return false;

            var hive = new RegistryHive(path) {RecoverDeleted = true};
            hive.ParseHive();

            var key = hive.GetKey("Setup");
            var val = key.Values.Single(x => x.ValueName == "OOBEInProgress");

            return int.Parse(val.ValueData) == 0;
        }

        private async Task<IPartition> GetWindowsPartition()
        {
            var deviceDisk = await GetDeviceDisk();
            return await deviceDisk.GetPartitionByName(PartitionName.Windows);
        }

        private static bool Contains(string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        private class DualBootStatus
        {
            public DualBootStatus(bool canDualBoot, bool isEnabled)
            {
                IsEnabled = isEnabled;
                CanDualBoot = canDualBoot;
            }

            public bool IsEnabled { get; }
            public bool CanDualBoot { get; }
        }

        private class BcdGuids
        {
            public static readonly Guid WinMobile = Guid.Parse("7619dcc9-fafe-11d9-b411-000476eba25f");
            public static readonly Guid Woa = Guid.Parse("7619dcca-fafe-11d9-b411-000476eba25f");
        }

        private static class PartitionName
        {
            public const string MainOs = "MainOS";
            public const string System = "SYSTEM";
            public const string Windows = "Windows";
            public const string Data = "Data";
        }
    }
}