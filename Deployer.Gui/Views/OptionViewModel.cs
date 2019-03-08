using System.Windows.Input;

namespace Deployer.Gui.Views
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