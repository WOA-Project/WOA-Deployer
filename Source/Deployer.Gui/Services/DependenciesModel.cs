using System.Collections.Generic;
using Deployer.Gui.ViewModels;
using Deployer.Gui.ViewModels.Common;

namespace Deployer.Gui.Services
{
    public class DependenciesModel
    {
        public IEnumerable<RequirementFiller> Children { get; set; }
    }
}