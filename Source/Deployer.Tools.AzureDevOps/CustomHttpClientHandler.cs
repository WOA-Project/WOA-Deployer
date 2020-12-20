using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Deployer.Tools.AzureDevOps
{
    public class CustomHttpClientHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}