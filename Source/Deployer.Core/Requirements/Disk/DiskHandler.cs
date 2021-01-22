using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Deployer.Core.Requirements.Disk
{
    // ReSharper disable once UnusedType.Global
    public class DiskHandler : IRequestHandler<DiskRequest, RequirementResponse>
    {
        public Task<RequirementResponse> Handle(DiskRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RequirementResponse(new[]
            {
                new FulfilledRequirement(request.Key, request.Index),
            }));
        }
    }
}