using System;
using System.Threading.Tasks;
using System.Windows;

namespace Deployer.UI.Views
{
    public static class AsyncWindowExtension
    {
        public static Task<bool?> ShowDialogAsync(this Window self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            var completion = new TaskCompletionSource<bool?>();
            self.Owner = Application.Current.MainWindow;
            self.Dispatcher.BeginInvoke(new Action(() => completion.SetResult(self.ShowDialog())));
            return completion.Task;
        }
    }
}