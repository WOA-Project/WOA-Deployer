using MediatR;

namespace Deployer.Core.Requirements.Disk
{
    public class DiskRequest : RequirementRequest, IRequest<RequirementResponse>
    {
        public int Index { get; set; }
    }
}