using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Deployer.Core.Compiler;
using Iridio.Binding;
using Iridio.Common;
using Iridio.Parsing;
using Zafiro.Core;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;
using Zafiro.Wpf;
using Zafiro.Wpf.Services;

namespace Editor.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            var fileSystemOperations = new FileSystemOperations();
            var preprocessor = new Preprocessor(fileSystemOperations);
            var deployerCompiler = new DeployerCompiler(preprocessor, new Parser(), new Binder(new BindingContext(new List<IFunction>())));
            Func<string, IZafiroFile> fileFactory = s => new DesktopZafiroFile(new Uri(s), fileSystemOperations,new Downloader(new HttpClient()));
            var openFilePicker = new OpenFilePicker(fileFactory, fileSystemOperations);
            DataContext = new MainViewModel(deployerCompiler, openFilePicker, new WpfDialogService());

            base.OnInitialized(e);
        }
    }
}
