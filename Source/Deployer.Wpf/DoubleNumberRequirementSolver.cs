using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Deployer.Core.Requirements;
using Deployer.Core.Requirements.Number;
using MediatR;
using ReactiveUI;

namespace Deployer.Wpf
{
    public class DoubleNumberRequirementSolver : ReactiveObject, IRequirementSolver
    {
        private readonly string key;
        private IMediator mediator;

        private double v;

        public DoubleNumberRequirementSolver(string key, double min, double defaultValue, double max, string description, IMediator mediator)
        {
            Min = min;
            Max = max;
            Description = description;
            this.mediator = mediator;
            this.key = key;
            Value = defaultValue;
            IsValid = Observable.Return(true);
        }

        public double Min { get; }
        public double Max { get; }
        public string Description { get; }

        public double Value
        {
            get => v;
            set => this.RaiseAndSetIfChanged(ref v, value);
        }

        public virtual IObservable<bool> IsValid { get; }

        public virtual async Task<RequirementResponse> FulfilledRequirements()
        {
            var req = new DoubleNumberRequest(key, Value)
            {
                Key = key
            };

            var fulfilledRequirements = await mediator.Send(req);
            return fulfilledRequirements;
        }
    }
}