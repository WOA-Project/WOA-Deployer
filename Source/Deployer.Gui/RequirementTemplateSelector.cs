using System.Windows;
using System.Windows.Controls;
using Deployer.Gui.Services;
using Deployer.Gui.ViewModels;
using Deployer.Gui.ViewModels.Common;
using Deployer.Gui.ViewModels.Common.Disk;
using Deployer.Gui.ViewModels.RequirementSolvers;

namespace Deployer.Gui
{
    public class RequirementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WimImageTemplate { get; set; }

        public DataTemplate DiskTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WimPickRequirementSolver)
            {
                return WimImageTemplate;
            }

            //if (item is DiskFillerViewModel)
            //{
            //    return DiskTemplate;
            //}
            
            //if (item is SdCardFillerViewModel)
            //{
            //    return DiskTemplate;
            //}

            return base.SelectTemplate(item, container);
        }
    }
}