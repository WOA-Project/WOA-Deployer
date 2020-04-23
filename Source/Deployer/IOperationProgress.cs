using System.Reactive.Subjects;

namespace Deployer
{
    public interface IOperationProgress
    {
        ISubject<double> Percentage { get; set; }
        ISubject<long> Value { get; set; }
    }
}