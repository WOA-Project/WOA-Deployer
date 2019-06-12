using Microsoft.Win32;

namespace Deployer.NetFx
{
    public class OS
    {
        public static bool IsCompatibleWindowsBuild => GetBuildNumber() >= 15063;

        public static int GetBuildNumber()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                if (key != null)
                {
                    var buildStr = (string)key.GetValue("CurrentBuild");
                    if (int.TryParse(buildStr, out var buildNumber))
                    {
                        return buildNumber;
                    }
                }
            }

            return 0;
        }
    }
}