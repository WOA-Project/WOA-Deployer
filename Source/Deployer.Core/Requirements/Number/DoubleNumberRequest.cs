using MediatR;

namespace Deployer.Core.Requirements.Number
{
    public class DoubleNumberRequest : RequirementRequest, IRequest<RequirementResponse>
    {
        public double Value { get; }

        public DoubleNumberRequest(string key, double value)
        {
            Value = value;
            Key = key;
        }
    }
}