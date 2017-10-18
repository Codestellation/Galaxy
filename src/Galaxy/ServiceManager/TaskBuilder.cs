using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TaskBuilder
    {
        private readonly OperationBuilder _operations;
        private readonly IMediator _mediator;

        public TaskBuilder(OperationBuilder operationBuilder, IMediator mediator)
        {
            _operations = operationBuilder;
            _mediator = mediator;
        }

        public DeploymentTask DeployServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("UpdateService", deployment, deploymentFeed)
                .Add(_operations.StopService())
                .Add(_operations.BackupService())
                .Add(_operations.ClearBinaries())
                .Add(_operations.EnsureFolders())
                .Add(_operations.InstallPackage())
                .Add(_operations.DeployHostConfig())
                .Add(_operations.GetConfigSample())
                .Add(_operations.DeployServiceConfig())
                .Add(_operations.StartService());
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("InstallService", deployment, feed)
                .Add(_operations.InstallService());
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("UninstallService", deployment, feed)
                .Add(_operations.StopService())
                .Add(_operations.UninstallService());
        }

        public DeploymentTask StartServiceTask(Deployment deployment, NugetFeed feed)
        {
            var startServiceTask = CreateDeployTask("StartService", deployment, feed)
                .Add(_operations.StartService());
            startServiceTask.Context.SetValue(DeploymentTaskContext.ForceStartService, true);
            return startServiceTask;
        }

        public DeploymentTask StopServiceTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("StopService", deployment, feed).Add(_operations.StopService());
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
                .Add(_operations.StopService())
                .Add(_operations.BackupService())
                .Add(_operations.ClearBinaries())
                .Add(_operations.RestoreFrom());
        }

        public DeploymentTask DeleteDeploymentTask(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("DeleteDeployment", deployment, feed, new MemoryStream(1024)) //We are going to delete directory where logs would be written. That's why we hack it!
                .Add(_operations.StopService())
                .Add(_operations.UninstallService())
                .Add(_operations.DeleteFolders())
                .Add(_operations.UninstallPackage())
                .Add(_operations.PublishDeletedEvent());
        }

        public DeploymentTask MoveFolder(Deployment deployment, NugetFeed feed)
        {
            return CreateDeployTask("Move Folder", deployment, feed)
                .Add(_operations.StopService())
                .Add(_operations.UninstallService())
                .Add(_operations.EnsureFolders())
                .Add(_operations.DeployHostConfig())
                .Add(_operations.DeployServiceConfig())
                .Add(_operations.StartService())
                .Add(_operations.InstallService());
        }
    }
}