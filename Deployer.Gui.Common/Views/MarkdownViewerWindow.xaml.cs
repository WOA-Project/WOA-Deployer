using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Deployer.Gui.Common.Views
{
    
    public partial class MarkdownViewerWindow : ICloseable
    {
        public MarkdownViewerWindow()
        {
            InitializeComponent();
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start((string) e.Parameter);
        }
    }
}
