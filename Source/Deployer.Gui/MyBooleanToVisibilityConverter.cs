using System.Windows;
using System.Windows.Data;
using ValueConverters;

namespace Deployer.Gui
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class MyBoolToVisibilityConverter : BoolToVisibilityConverter
    {
    }
}