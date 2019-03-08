namespace Deployer.Gui
{
    public class UIServices
    {
        public UIServices(IFilePicker filePicker, IViewService viewService, IDialog dialog)
        {
            FilePicker = filePicker;
            ViewService = viewService;
            Dialog = dialog;
        }

        public IFilePicker FilePicker { get; }
        public IViewService ViewService { get; }
        public IDialog Dialog { get; }
    }
}