using System.Threading.Tasks;

namespace Deployer.UI
{
    public interface IContextDialog
    {
        Task ShowAlert(object owner, string title, string text);
        Task<DialogResult> ShowOkCancel(object owner, string title, string text);
        Task<DialogResult> ShowYesNo(object owner, string title, string text, bool isYesDefault = true);
    }
}