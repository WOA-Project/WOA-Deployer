using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;

namespace Deployer.Gui.ViewModels.Disk
{
    public class SdCardFillerViewModel : DiskFillerViewModelBase
    {
        public SdCardFillerViewModel(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public override string AssociatedRequirement => Requirement.SdCard;
    }
}