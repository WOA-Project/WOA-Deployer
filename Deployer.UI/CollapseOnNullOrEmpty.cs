using System;
using System.Globalization;
using System.Windows;
using ValueConverters;

namespace Deployer.UI
{
    public class CollapseOnNullOrEmpty : StringIsNotNullOrEmptyConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool v)
            {
                return v  ? Visibility.Visible : Visibility.Collapsed;
            }

            return false;
        }
    }
}