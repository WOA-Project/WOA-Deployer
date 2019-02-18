namespace Deployer.Gui.Common.Views
{
    public class Option
    {
        public string Name { get; }
        public DialogValue DialogValue { get; }

        public Option(string name, DialogValue dialogValue = DialogValue.None)
        {
            Name = name;
            DialogValue = dialogValue;
        }
    }

    public enum DialogValue
    {
        None,
        // ReSharper disable once InconsistentNaming
        OK,
        Cancel
    }
}