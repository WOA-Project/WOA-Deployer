using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;

namespace Deployer.Gui.ViewModels.Common.Disk
{
    public class DiskFillerViewModel : DiskFillerViewModelBase
    {
        public DiskFillerViewModel(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public override string AssociatedRequirement => Requirement.Disk;
    }
}