using System.Windows.Data;
using ValueConverters;

namespace Deployer.Wpf
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class MyNullToBoolConverter : NullToBoolConverter
    {
    }
}