using MediatR;

namespace Deployer.Core.Requirements.Number
{
    public class NumberRequest : RequirementRequest, IRequest<RequirementResponse>
    {
        public double Value { get; }

        public NumberRequest(string key, double value)
        {
            Value = value;
            Key = key;
        }
    }
}