using System.Windows.Data;
using ValueConverters;

namespace Deployer.Gui
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class MyNullToBoolConverter : NullToBoolConverter
    {
    }
}