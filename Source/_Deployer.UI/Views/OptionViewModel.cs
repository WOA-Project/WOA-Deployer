using System.Windows.Input;
using Deployer.Core;

namespace Deployer.UI.Views
{
    public class OptionViewModel
    {
        public Option Option { get; }

        public OptionViewModel(Option option)
        {
            Option = option;
        }

        public ICommand Command { get; set; }
    }
}