using System;
using System.Linq;
using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
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
        private readonly FeedBoard _feedBoard;
        private readonly DeploymentTaskProcessor _taskProcessor;
        private readonly TaskBuilder _builder;

        public ServiceEventsHandler(DeploymentBoard deploymentBoard, FeedBoard feedBoard, DeploymentTaskProcessor taskProcessor, TaskBuilder builder)
        {
            _deploymentBoard = deploymentBoard;
            _feedBoard = feedBoard;
            _taskProcessor = taskProcessor;
            _builder = builder;
        }

        public void Handle(InstallServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.InstallServiceTask(d, f));
        }

        public void Handle(UninstallServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.UninstallServiceTask(d, f));
        }

        public void Handle(StartServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.StartServiceTask(d, f));
        }

        public void Handle(StopServiceEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.StopServiceTask(d, f));
        }

        public void Handle(DeleteDeploymentEvent message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.DeleteDeploymentTask(d, f));
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

            var targetFeed = _feedBoard.Feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));

            var task = buildTask(deployment, targetFeed);

            _taskProcessor.Process(task);
        }
    }
}