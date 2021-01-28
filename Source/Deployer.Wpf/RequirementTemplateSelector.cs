using System.Windows;
using System.Windows.Controls;

namespace Deployer.Wpf
{
    public class RequirementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WimImageTemplate { get; set; }

        public DataTemplate DiskTemplate { get; set; }

        public DataTemplate DoubleTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WimPickRequirementSolver)
            {
                return WimImageTemplate;
            }

            if (item is DiskRequirementSolver)
            {
                return DiskTemplate;
            }


            if (item is DoubleNumberRequirementSolver)
            {
                return DoubleTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}