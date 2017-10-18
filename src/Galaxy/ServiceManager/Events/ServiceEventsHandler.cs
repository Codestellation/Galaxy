using System;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using MediatR;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class ServiceEventsHandler :
        IRequestHandler<DeleteDeploymentRequest>,
        IRequestHandler<InstallServiceRequest>,
        IRequestHandler<UninstallServiceRequest>,
        IRequestHandler<StartServiceRequest>,
        IRequestHandler<StopServiceRequest>,
        IRequestHandler<DeployServiceRequest>
    {
        private readonly DeploymentTaskProcessor _taskProcessor;
        private readonly TaskBuilder _builder;
        private readonly Collection _feedCollection;
        private readonly Collection _deployments;

        public ServiceEventsHandler(Repository repository, DeploymentTaskProcessor taskProcessor, TaskBuilder builder)
        {
            _feedCollection = repository.Feeds;
            _deployments = repository.Deployments;

            _taskProcessor = taskProcessor;
            _builder = builder;
        }

        public void Handle(InstallServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.InstallServiceTask(d));
        }

        public void Handle(UninstallServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.UninstallServiceTask(d));
        }

        public void Handle(StartServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.StartServiceTask(d));
        }

        public void Handle(StopServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.StopServiceTask(d));
        }

        public void Handle(DeleteDeploymentRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.DeleteDeploymentTask(d));
        }

        public void Handle(DeployServiceRequest message)
        {
            using (var tx = _deployments.BeginTransaction())
            {
                var deployment = _deployments.Load<Deployment>(message.DeploymentId);
                deployment.PackageVersion = message.Version;
                _deployments.Save(deployment, false);
                tx.Commit();
            }
            BuildAndProcess(message.DeploymentId, (d, f) => _builder.DeployServiceTask(d, f));
        }

        private void BuildAndProcess(ObjectId deploymentId, Func<Deployment, NugetFeed, DeploymentTask> buildTask)
        {
            var deployment = _deployments.Load<Deployment>(deploymentId);

            var feeds = _feedCollection.PerformQuery<NugetFeed>();
            var targetFeed = feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));

            var task = buildTask(deployment, targetFeed);

            _taskProcessor.Process(task);
        }
    }
}