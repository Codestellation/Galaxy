using System;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TaskBuilder
    {
        private readonly OperationBuilder _operations;

        public TaskBuilder(OperationBuilder operationBuilder)
        {
            _operations = operationBuilder;
        }

        public DeploymentTask DeployServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("DeployService", deployment)
                .Add(_operations.BackupService(deployment))
                .Add(_operations.InstallPackage(deployment, deploymentFeed, FileList.Empty))
                .Add(_operations.OverrideFiles(deployment));
        }

        public DeploymentTask UpdateServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("UpdateService", deployment)
                .Add(_operations.StopService(deployment))
                .Add(_operations.BackupService(deployment))
                .Add(_operations.ClearBinaries(deployment))
                .Add(_operations.InstallPackage(deployment, deploymentFeed, deployment.KeepOnUpdate.Clone()))
                .Add(_operations.OverrideFiles(deployment));
        }

        public DeploymentTask InstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("InstallService", deployment)
                .Add(_operations.InstallService(deployment));
        }

        public DeploymentTask UninstallServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("UninstallService", deployment)
                .Add(_operations.StopService(deployment))
                .Add(_operations.UninstallService(deployment));
        }

        public DeploymentTask StartServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("StartService", deployment)
                .Add(_operations.StartService(deployment));
        }

        public DeploymentTask StopServiceTask(Deployment deployment, NugetFeed deploymentFeed)
        {
            return CreateDeployTask("StopService", deployment)
                .Add(_operations.StopService(deployment));
        }

        private static DeploymentTask CreateDeployTask(string name, Deployment deployment)
        {
            var deployLogFolder = deployment.GetDeployLogFolder();
            Folder.EnsureExists(deployLogFolder);
            
            var filename = string.Format("{0}.{1:yyyy-MM-dd_HH.mm.ss}.log", name, Clock.UtcNow.ToLocalTime());
            var fullPath = Path.Combine(deployLogFolder, filename);
            var logStream = File.Open(fullPath, FileMode.Create, FileAccess.Write);
            
            return new DeploymentTask(name, deployment.Id, logStream);
        }
    }
}