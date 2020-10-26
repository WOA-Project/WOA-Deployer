using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Deployer.Core.Requirements
{
    public class WimFileHandler : IRequestHandler<WimFileRequest, RequirementResponse>
    {
        public Task<RequirementResponse> Handle(WimFileRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RequirementResponse(new[]
            {
                new FulfilledRequirement(request.Key + "Path", request.Path),
                new FulfilledRequirement(request.Key + "Index", request.Index),
            }));
        }
    }
}