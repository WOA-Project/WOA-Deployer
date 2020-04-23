namespace Deployer
{
    public class Option
    {
        public string Name { get; }
        public OptionValue OptionValue { get; }

        public Option(string name, OptionValue optionValue = OptionValue.None)
        {
            Name = name;
            OptionValue = optionValue;
        }
    }
}