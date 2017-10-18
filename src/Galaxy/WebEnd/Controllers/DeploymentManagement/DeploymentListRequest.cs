using Codestellation.Galaxy.Infrastructure;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement
{
    public class DeploymentListRequest : IRequest<DeploymentListResponse>, IMainRequest
    {
    }
}