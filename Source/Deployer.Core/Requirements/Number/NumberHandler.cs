using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Deployer.Core.Requirements.Number
{
    // ReSharper disable once UnusedType.Global
    public class NumberHandler : IRequestHandler<NumberRequest, RequirementResponse>
    {
        public Task<RequirementResponse> Handle(NumberRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RequirementResponse(new[]
            {
                new FulfilledRequirement(request.Key, request.Value),
            }));
        }
    }
}