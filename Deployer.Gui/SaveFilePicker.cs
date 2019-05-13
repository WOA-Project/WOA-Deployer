using System.Windows;
using Microsoft.Win32;

namespace Deployer.UI
{
    public class SaveFilePicker : ISaveFilePicker
    {
        public string Filter { get; set; }
        public string FileName { get; set; }
        public string DefaultExt { get; set; }

        public string PickFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = Filter,
                DefaultExt = DefaultExt,
                FileName = ""
            };
            
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return dialog.FileName;
            }

            return null;
        }       
    }
}