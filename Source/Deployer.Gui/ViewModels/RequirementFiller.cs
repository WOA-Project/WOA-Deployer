using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;

namespace Deployer.Gui.ViewModels
{
    public abstract class RequirementFiller : ReactiveObject
    {
        public abstract IEnumerable<string> HandledRequirements { get; }
        public Requirements Requirements { get; set; }
    }

    public class Requirements : ReactiveObject
    {
        private readonly IDictionary<string, object> dict;

        public Requirements(IDictionary<string, object> dict)
        {
            this.dict = dict;
            Satisfied = this.WhenAnyPropertyChanged().Select(te => te.dict.Values.All(v => v != null));
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