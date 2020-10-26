using MediatR;

namespace Deployer.Core.Requirements
{
    public class WimFileRequest : RequirementRequest, IRequest<RequirementResponse>
    {
        public string Path { get; set; }
        public int Index { get; set; }
    }
}