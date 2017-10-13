using System;
using System.Linq;
using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class ServiceEventsHandler :
        IHandler<DeleteDeploymentEvent>,
        IHandler<InstallServiceEvent>,
        IHandler<UninstallServiceEvent>,
        IHandler<StartServiceEvent>,
        IHandler<StopServiceEvent>,
        IHandler<DeployServiceEvent>
    {
        private readonly DeploymentBoard _deploymentBoard;
        private readonly DeploymentTaskProcessor _taskProcessor;
        private readonly TaskBuilder _builder;
        private readonly Collection _feedCollection;

        public ServiceEventsHandler(Repository repository, DeploymentBoard deploymentBoard, DeploymentTaskProcessor taskProcessor, TaskBuilder builder)
        {
            _feedCollection = repository.GetCollection<NugetFeed>();
            _deploymentBoard = deploymentBoard;
            _taskProcessor = taskProcessor;
            _builder = builder;
        }

        public void Handle(InstallServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.InstallServiceTask(d));
        }

        public void Handle(UninstallServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.UninstallServiceTask(d));
        }

        public void Handle(StartServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.StartServiceTask(d));
        }

        public void Handle(StopServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.StopServiceTask(d));
        }

        public void Handle(DeleteDeploymentEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.DeleteDeploymentTask(d));
        }

        public void Handle(DeployServiceEvent message)
        {
            var deployment = _deploymentBoard.GetDeployment(message.DeploymentId);
            deployment.PackageVersion = message.Version;

            _deploymentBoard.SaveDeployment(deployment);

            BuildAndProcess(message.DeploymentId, (d, f) => _builder.DeployServiceTask(d, f));
        }

        private void BuildAndProcess(ObjectId deploymentId, Func<Deployment, NugetFeed, DeploymentTask> buildTask)
        {
            var deployment = _deploymentBoard.GetDeployment(deploymentId);

            var feeds = _feedCollection.PerformQuery<NugetFeed>();
            var targetFeed = feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));

            var task = buildTask(deployment, targetFeed);

            _taskProcessor.Process(task);
        }
    }
}