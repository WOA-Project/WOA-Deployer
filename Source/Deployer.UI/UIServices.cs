using System;
using Deployer.Core;
using Deployer.Core.Interaction;
using Zafiro.Core.UI;

namespace Deployer.UI
{
    public class UIServices
    {
        public UIServices(IOpenFilePicker openFilePicker, ISaveFilePicker saveFilePicker, IViewService viewService,
            Func<object, IDialogService> dialogFactory)
        {
            OpenFilePicker = openFilePicker;
            SaveFilePicker = saveFilePicker;
            ViewService = viewService;
            DialogFactory = dialogFactory;
        }

        public IOpenFilePicker OpenFilePicker { get; }
        public ISaveFilePicker SaveFilePicker { get; }
        public IViewService ViewService { get; }
        public Func<object, IDialogService> DialogFactory { get; }
    }
}