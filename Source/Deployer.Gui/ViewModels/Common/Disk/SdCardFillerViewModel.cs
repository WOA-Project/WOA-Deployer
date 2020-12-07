using Deployer.Core.Scripting.Core;
using Deployer.Filesystem;

namespace Deployer.Gui.ViewModels.Common.Disk
{
    public class SdCardFillerViewModel : DiskFillerViewModelBase
    {
        public SdCardFillerViewModel(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public override string AssociatedRequirement => Requirement.SdCard;
    }
}