using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Deployer.Core.FileSystem;
using Deployer.Core.Scripting.Core;
using ReactiveUI;

namespace Deployer.Gui.ViewModels.Disk
{
    public class DiskFillerViewModel : DiskFillerViewModelBase
    {
        public DiskFillerViewModel(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public override string AssociatedRequirement => Requirement.Disk;
    }
}