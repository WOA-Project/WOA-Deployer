using Deployer.Core;
using Deployer.Gui.New;

namespace Deployer.Gui
{
    public class AppProperties
    {
        public const string GitHubBaseUrl = "https://github.com/WOA-Project/WOA-Deployer";
        public static string AppTitle => string.Format(Resources.AppTitle, AppVersionMixin.VersionString);
    }
}