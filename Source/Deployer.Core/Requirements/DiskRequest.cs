using MediatR;

namespace Deployer.Core.Requirements
{
    public class DiskRequest : RequirementRequest, IRequest<RequirementResponse>
    {
        public int Index { get; set; }
    }
}