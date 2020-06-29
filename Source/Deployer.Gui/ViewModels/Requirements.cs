using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;

namespace Deployer.Gui.ViewModels
{
    public class Requirements : ReactiveObject
    {
        private readonly IDictionary<string, object> dict;

        public Requirements(IDictionary<string, object> dict)
        {
            this.dict = dict;
            Satisfied = this.WhenAnyPropertyChanged().Select(te => Enumerable.All<object>(te.dict.Values, v => v != null));
        }

        public IObservable<bool> Satisfied { get; }

        public object this[string token]
        {
            get { return dict[token]; }
            set
            {
                if (Equals(dict[token], value))
                {
                    return;
                }

                dict[token] = value;
                this.RaisePropertyChanged(token);
            }
        }
    }
}