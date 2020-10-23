using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Deployer.Core;
using Deployer.Core.Compiler;
using Deployer.Core.Registrations;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Parsing;
using Optional;
using Uno.Extensions;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Core.UI;
using Zafiro.Uwp.Controls;
using Option = Zafiro.Core.Option;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CodeEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var fileSystemOperations = new FileSystemOperations();
            var preprocessor = new Preprocessor(fileSystemOperations);
            var deployerCompiler = new DeployerCompiler(preprocessor, new Parser(), new Binder(new BindingContext(new List<IFunction>())));
            DataContext = new MainViewModel(deployerCompiler, new UwpOpenPicker(), new UwpDialogService());
        }
    }

    public class UwpDialogService : IDialogService
    {
        public Task Notice(string title, string content)
        {
            throw new NotImplementedException();
        }

        public Task<Option> Interaction(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "")
        {
            throw new NotImplementedException();
        }
    }

    public class UwpOpenPicker : IOpenFilePicker
    {
        public string InitialDirectory { get; set; }
        public List<FileTypeFilter> FileTypeFilter { get; set; } = new List<FileTypeFilter>();
        public async Task<Option<IZafiroFile>> Pick()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Clear();
            picker.FileTypeFilter.AddRange(FileTypeFilter.SelectMany(s => s.Extensions));
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            var file = (await picker.PickSingleFileAsync()).SomeNotNull();
            return file.Map(storageFile => (IZafiroFile)new UwpFile(storageFile));
        }
    }
}
