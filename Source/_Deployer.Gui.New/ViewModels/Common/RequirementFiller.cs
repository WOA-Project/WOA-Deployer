using System.Collections.Generic;
using ReactiveUI;

namespace Deployer.Gui.ViewModels.Common
{
    public abstract class RequirementFiller : ReactiveObject
    {
        public abstract IEnumerable<string> HandledRequirements { get; }
        public Requirements Requirements { get; set; }
    }
}