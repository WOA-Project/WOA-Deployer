using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deployer
{
    public static class DialogMixin
    {
        public static Task<Option> Pick(this IDialog dialog, string message, IEnumerable<Option> options, string assetFolder = "")
        {
            return dialog.Pick("", message, options, assetFolder);
        }

        public static Task ShowMessage(this IDialog dialog, string title, string message, string assetFolder = "")
        {
            return dialog.Pick(title, message, new List<Option>() {new Option("OK", OptionValue.OK)}, assetFolder);
        }
    }
}