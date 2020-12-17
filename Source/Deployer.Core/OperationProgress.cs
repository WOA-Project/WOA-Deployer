using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Zafiro.Core;

namespace Deployer.Core
{
    public class OperationProgress : IOperationProgress
    {
        private readonly ISubject<Progress> progress = new BehaviorSubject<Progress>(new Done());
        public ISubject<double> Percentage { get; set; } = new BehaviorSubject<double>(double.NaN);
        public ISubject<long> Value { get; set; } = new BehaviorSubject<long>(0L);
        public void Send(Progress current)
        {
            progress.OnNext(current);
        }

        public IObservable<Progress> Progress => progress.AsObservable();
    }
}