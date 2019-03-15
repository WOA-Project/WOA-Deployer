using System.Windows;
using System.Windows.Controls;

namespace Deployer.Gui
{
    public class ProgressButton : ContentControl
    {
        static ProgressButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressButton), new FrameworkPropertyMetadata(typeof(ProgressButton)));
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(object), typeof(ProgressButton), new PropertyMetadata(default(object)));

        public object Icon
        {
            get { return (object) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
            "Progress", typeof(ProgressViewModel), typeof(ProgressButton), new PropertyMetadata(default(ProgressViewModel)));

        public ProgressViewModel Progress
        {
            get { return (ProgressViewModel) GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
    }
}
