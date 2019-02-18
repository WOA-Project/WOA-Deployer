using System.Collections.Generic;

namespace Deployer.Gui.Common.Views
{
    public class MessageViewModel
    {
        public string Title { get; }
        public string Text { get; }

        public ICollection<Option> Options { get; set; }

        public MessageViewModel(string title, string text)
        {
            Title = title;
            Text = text;
        }
    }
}