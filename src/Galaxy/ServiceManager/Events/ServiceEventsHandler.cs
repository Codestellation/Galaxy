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
        IRequestHandler<DeployServiceRequest>,
        IRequestHandler<RestoreServiceRequest>
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
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.InstallServiceTask(d, f));
        }

        public void Handle(UninstallServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.UninstallServiceTask(d, f));
        }

        public void Handle(StartServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.StartServiceTask(d, f));
        }

        public void Handle(StopServiceRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.StopServiceTask(d, f));
        }

        public void Handle(DeleteDeploymentRequest message)
        {
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.DeleteDeploymentTask(d, f));
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
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.DeployServiceTask(d, f));
        }

        public void Handle(RestoreServiceRequest message)
        {
            var parameters = new { message.RestoreFrom };
            BuildAndProcess(message.DeploymentId, (d, f, p) => _builder.RestoreFromBackup(d, f, p), parameters);
        }

        private void BuildAndProcess(ObjectId deploymentId, Func<Deployment, NugetFeed, object, DeploymentTask> buildTask, object parameters = null)
        {
            var deployment = _deployments.Load<Deployment>(deploymentId);

            var feeds = _feedCollection.PerformQuery<NugetFeed>();
            var targetFeed = feeds.FirstOrDefault(item => item.Id.Equals(deployment.FeedId));

            var task = buildTask(deployment, targetFeed, parameters);

            _taskProcessor.Process(task);
        }
    }
}