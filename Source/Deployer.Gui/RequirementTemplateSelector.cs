using System.Windows;
using System.Windows.Controls;
using Deployer.Gui.Services;
using Deployer.Gui.ViewModels;
using Deployer.Gui.ViewModels.Disk;
using Deployer.Lumia.Gui.Views.Parts;

namespace Deployer.Gui
{
    public class RequirementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WimImageTemplate { get; set; }

        public DataTemplate DiskTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WimPickViewModel)
            {
                return WimImageTemplate;
            }

            if (item is DiskFillerViewModel)
            {
                return DiskTemplate;
            }
            
            if (item is SdCardFillerViewModel)
            {
                return DiskTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}