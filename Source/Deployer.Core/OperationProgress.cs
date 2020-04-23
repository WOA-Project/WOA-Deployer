using System.Reactive.Subjects;

namespace Deployer.Core
{
    public class OperationProgress : IOperationProgress
    {
        public ISubject<double> Percentage { get; set; } = new BehaviorSubject<double>(double.NaN);
        public ISubject<long> Value { get; set; } = new BehaviorSubject<long>(0L);
    }
}