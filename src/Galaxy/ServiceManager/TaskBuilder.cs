using System.IO;
using Castle.MicroKernel;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TaskBuilder
    {
        private readonly IMediator _mediator;
        private readonly IKernel _kernel;

        public TaskBuilder(IMediator mediator, IKernel kernel)
        {
            _mediator = mediator;
            _kernel = kernel;
        }

        public DeploymentTask DeployServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("UpdateService", deployment, deploymentFeed)
                .Add(Operation<StopService>())
                .Add(Operation<BackupService>())
                .Add(Operation<ClearBinaries>())
                .Add(Operation<EnsureFolders>())
                .Add(Operation<InstallPackage>())
                .Add(Operation<DeployHostConfig>())
                .Add(Operation<GetConfigSample>())
                .Add(Operation<DeployServiceConfig>())
                .Add(Operation<StartService>());
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("InstallService", deployment, feed)
                .Add(Operation<InstallService>());
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("UninstallService", deployment, feed)
                .Add(Operation<StopService>())
                .Add(Operation < UninstallService>());
        }

        public DeploymentTask StartServiceTask(Deployment deployment, NugetFeed feed)
        {
            var startServiceTask = CreateDeployTask("StartService", deployment, feed)
                .Add(Operation<StartService>());
            startServiceTask.Context.SetValue(DeploymentTaskContext.ForceStartService, true);
            return startServiceTask;
        }

        public DeploymentTask StopServiceTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("StopService", deployment, feed).Add(Operation<StopService>());
        }

        private DeploymentTask CreateDeployTask(string name, Deployment deployment, NugetFeed deploymentFeed, object parameters = null, Stream logStream = null)
        {
            var actualLogStream = logStream ?? BuildDefaultLogStream(name, deployment);
            var streamWriter = new StreamWriter(actualLogStream);

            var context = new DeploymentTaskContext(streamWriter)
            {
                Parameters = parameters ?? new object(),
                DeploymentId = deployment.Id,
                Folders = deployment.Folders,
                ServiceFileName = $"{deployment.PackageId}.exe",
                InstanceName = deployment.InstanceName,
                ServiceName = string.IsNullOrWhiteSpace(deployment.InstanceName)
                    ? deployment.PackageId
                    : $"{deployment.PackageId}${deployment.InstanceName}",
                PackageDetails = new PackageDetails(deployment.PackageId, deploymentFeed.Uri, deployment.PackageVersion)
            };
            context
                .SetValue(DeploymentTaskContext.TaskName, name)
                .SetValue(DeploymentTaskContext.PublisherKey, _mediator)
                .SetValue(DeploymentTaskContext.LogStream, actualLogStream)
                .SetValue(DeploymentTaskContext.Config, deployment.Config);

            return new DeploymentTask(context);
        }

        private static FileStream BuildDefaultLogStream(string name, Deployment deployment)
        {
            FullPath deployLogFolder = deployment.Folders.DeployFolder;
            Folder.EnsureExists((string)deployLogFolder);

            var filename = $"{name}.{Clock.UtcNow.ToLocalTime():yyyy-MM-dd_HH.mm.ss}.log";
            var fullPath = Path.Combine((string)deployLogFolder, filename);
            var defaultStream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            return defaultStream;
        }

        public DeploymentTask RestoreFromBackup(Deployment deployment, NugetFeed feed, object parameters = null)
        {
            return CreateDeployTask("Restore From Backup", deployment, feed, parameters)
                .Add(Operation<StopService>())
                .Add(Operation<BackupService>())
                .Add(Operation<ClearBinaries>())
                .Add(Operation<RestoreFromBackup>());
        }

        public DeploymentTask DeleteDeploymentTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("DeleteDeployment", deployment, feed, new MemoryStream(1024)) //We are going to delete directory where logs would be written. That's why we hack it!
                .Add(Operation<StopService>())
                .Add(Operation<UninstallService>())
                .Add(Operation<DeleteFolders>())
                .Add(Operation<UninstallPackage>())
                .Add(Operation<PublishDeploymentDeletedEvent>());
        }

        public DeploymentTask MoveFolder(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("Move Folder", deployment, feed)
                .Add(Operation<StopService>())
                .Add(Operation<UninstallService>())
                .Add(Operation<EnsureFolders>())
                .Add(Operation<DeployHostConfig>())
                .Add(Operation<DeployServiceConfig>())
                .Add(Operation<StartService>())
                .Add(Operation<InstallService>());
        }

        private IOperation Operation<TOperation>()
            where TOperation : IOperation
        {
            return _kernel.Resolve<TOperation>();
        }
    }
}