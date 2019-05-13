namespace Deployer.UI
{
    public class UIServices
    {
        public UIServices(IOpenFilePicker openFilePicker, ISaveFilePicker saveFilePicker, IViewService viewService,
            IContextDialog contextDialog, IDialog dialog)
        {
            OpenFilePicker = openFilePicker;
            SaveFilePicker = saveFilePicker;
            ViewService = viewService;
            ContextDialog = contextDialog;
            Dialog = dialog;
        }

        public IOpenFilePicker OpenFilePicker { get; }
        public ISaveFilePicker SaveFilePicker { get; }
        public IViewService ViewService { get; }
        public IContextDialog ContextDialog { get; }
        public IDialog Dialog { get; }
    }
}