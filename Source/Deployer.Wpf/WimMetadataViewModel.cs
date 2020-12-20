using System;
using System.Collections.Generic;
using System.Linq;
using Deployer.Tools.Wim;
using ReactiveUI;
using Zafiro.Core.Patterns.Either;

namespace Deployer.Wpf
{
    public class WimMetadataViewModel : ReactiveObject
    {
        public string Errors { get; }
        private DiskImageMetadata selectedDiskImage;

        public WimMetadataViewModel(XmlWindowsImageMetadata windowsImageMetadata, string path)
        {
            WindowsImageMetadata = windowsImageMetadata;
            Path = path;
            SelectedImageObs = this.WhenAnyValue(x => x.SelectedDiskImage);
            SelectedDiskImage = Images.First();
        }

        public WimMetadataViewModel(ErrorList errors)
        {
            Errors = string.Join(";", errors);
        }

        private XmlWindowsImageMetadata WindowsImageMetadata { get; } = new XmlWindowsImageMetadata();

        public string Path { get; }

        public IObservable<DiskImageMetadata> SelectedImageObs { get; }

        public ICollection<DiskImageMetadata> Images => WindowsImageMetadata.Images;

        public DiskImageMetadata SelectedDiskImage
        {
            get => selectedDiskImage;
            set => this.RaiseAndSetIfChanged(ref selectedDiskImage, value);
        }
    }
}