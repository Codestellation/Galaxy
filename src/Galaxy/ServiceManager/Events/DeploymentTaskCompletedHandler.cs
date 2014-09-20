using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedHandler : IHandler<DeploymentTaskCompletedEvent>
    {
        private readonly DashBoard _dashBoard;
        private readonly Repository _repository;
        private readonly PackageVersionCache _versionCache;

        public DeploymentTaskCompletedHandler(DashBoard dashBoard, Repository repository, PackageVersionCache versionCache)
        {
            _dashBoard = dashBoard;
            _repository = repository;
            _versionCache = versionCache;
        }

        public void Handle(DeploymentTaskCompletedEvent message)
        {
            var deployment = _dashBoard.GetDeployment(message.Task.DeploymentId);
            deployment.Status = message.Result.Details;
            SaveDeployment(deployment);
        }

        private void SaveDeployment(Deployment deployment)
        {
            var deployments = _repository.GetCollection<Deployment>();

            using (var tx = deployments.BeginTransaction())
            {
                deployments.Save(deployment, false);
                tx.Commit();
            }
            _versionCache.ForceRefresh();
        }
    }
}