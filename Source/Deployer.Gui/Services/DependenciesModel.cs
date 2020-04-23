using System.Collections.Generic;
using Deployer.Gui.ViewModels;

namespace Deployer.Gui.Services
{
    public class DependenciesModel
    {
        public IEnumerable<RequirementFiller> Children { get; set; }
    }
}