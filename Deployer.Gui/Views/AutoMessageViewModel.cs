using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace Deployer.Gui.Views
{
    public class AutoMessageViewModel : ReactiveObject, IDisposable
    {
        private Option selectedOption;
        private readonly IDisposable closer;
        public string Text { get; }

        public AutoMessageViewModel(string text, IEnumerable<Option> options, ICloseable closeable)
        {
            Text = text;
            Options = options.Select(x =>
            {
                return new OptionViewModel(x)
                {
                    Command = ReactiveCommand.Create<OptionViewModel>(o => SelectedOption = x)
                };
            }).ToList();

            closer = this.WhenAnyValue(model => model.SelectedOption)
                .Where(s => s != null)
                .Subscribe(_ => closeable.Close());
        }

        public Option SelectedOption
        {
            get => selectedOption;
            set => this.RaiseAndSetIfChanged(ref selectedOption, value);
        }

        public List<OptionViewModel> Options { get; set; }

        public void Dispose()
        {
            closer?.Dispose();
        }
    }
}