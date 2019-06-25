namespace Deployer.Execution
{
    public class Argument
    {
        public Argument(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public override string ToString()
        {
            if (Value == null)
            {
                return "{null}";
            }
            else if (Value is string str)
            {
                 return $"\"{str}\"";
            }
            else if (Value.GetType().IsValueType)
            {
                return $"{{{Value}}}".ToLowerInvariant();
            }
            
            return Value.ToString();
        }
    }
}