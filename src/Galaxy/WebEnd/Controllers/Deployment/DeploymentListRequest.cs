using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class DeploymentListRequest : IRequest<DeploymentListResponse>, IMainRequest
    {
    }
}