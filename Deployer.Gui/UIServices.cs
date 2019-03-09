namespace Deployer.Gui
{
    public class UIServices
    {
        public UIServices(IOpenFilePicker openFilePicker, ISaveFilePicker saveFilePicker, IViewService viewService, IDialog dialog)
        {
            OpenFilePicker = openFilePicker;
            SaveFilePicker = saveFilePicker;
            ViewService = viewService;
            Dialog = dialog;
        }

        public IOpenFilePicker OpenFilePicker { get; }
        public ISaveFilePicker SaveFilePicker { get; }
        public IViewService ViewService { get; }
        public IDialog Dialog { get; }
    }
}