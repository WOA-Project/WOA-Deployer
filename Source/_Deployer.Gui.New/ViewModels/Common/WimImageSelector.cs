using System.Reactive;
using System.Reactive.Linq;
using Deployer.Gui.New.ViewModels.Common;
using ReactiveUI;

namespace Deployer.Gui.ViewModels.Common
{
    public class WimImageSelector : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<WimImage> selectedImage;

        public WimImageSelector(Commands commands)
        {
            PickImage = commands.PickWimFileCommand;

            selectedImage = PickImage
                .Select(model => new WimImage(model.Path, model.SelectedDiskImage.Index))
                .ToProperty(this, w => w.SelectedImage);
        }

        public ReactiveCommand<Unit, WimMetadataViewModel> PickImage { get; }

        public WimImage SelectedImage => selectedImage.Value;
    }
}