using System.Windows;
using System.Windows.Data;
using ValueConverters;

namespace Deployer.Wpf
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class MyBoolToVisibilityConverter : BoolToVisibilityConverter
    {
    }
}