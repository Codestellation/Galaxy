using System;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Host;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Quarks.IO;
using MediatR;

namespace Codestellation.Galaxy.WebEnd.Controllers.Deployment
{
    public class CreateDeploymentHandler : IRequestHandler<CreateDeploymentRequest>
    {
        private readonly Repository _repository;
        private readonly HostConfig _config;

        public CreateDeploymentHandler(Repository repository,  HostConfig config)
        {
            _repository = repository;
            _config = config;
        }
        public void Handle(CreateDeploymentRequest message)
        {
            var deployment = message.Model.ToDeployment();
            if (!deployment.Id.IsEmpty)
            {
                throw new InvalidOperationException("Deployment must be new deployment");
            }

            var options = _repository.Options.PerformQuery<Options>().Single();

            FillServiceFolders(deployment, options);

            SaveDeployment(deployment);

        }

        private void FillServiceFolders(Domain.Deployment deployment, Options options)
        {
            var serviceFolders = deployment.Folders;

            var subfolder = BuildSubfolder(deployment);

            var deployFolderPath = Folder.Combine(options.GetDeployFolder(), subfolder);
            serviceFolders.DeployFolder = (FullPath)deployFolderPath;

            BuildFolders(deployment, options);
        }

        private void SaveDeployment(Domain.Deployment deployment)
        {
            using (var tx = _repository.Deployments.BeginTransaction())
            {
                _repository.Deployments.Save(deployment, false);
                tx.Commit();
            }
        }

        private void BuildFolders(Domain.Deployment deployment, Options options)
        {
            string subFolder = BuildSubfolder(deployment);
            deployment.Folders.DeployLogsFolder = (FullPath)Folder.Combine(_config.Logs.FullName, "deploy-logs", subFolder);
            deployment.Folders.BackupFolder = (FullPath)Folder.Combine(_config.Data.FullName, "backups", subFolder);
            deployment.Folders.Logs = (FullPath)Folder.Combine(options.FolderOptions.Logs, subFolder);
            deployment.Folders.Configs = (FullPath)Folder.Combine(options.FolderOptions.Configs, subFolder);
            deployment.Folders.Data = (FullPath)Folder.Combine(options.FolderOptions.Data, subFolder);
        }

        private static string BuildSubfolder(Domain.Deployment deployment)
        {
            string subFolder = string.Empty;
            if (!string.IsNullOrWhiteSpace(deployment.Group))
            {
                subFolder = deployment.Group.Replace(" ", "-").ToLowerInvariant();
            }

            var name = (deployment.HasInstanceName
                    ? $"{deployment.PackageId}-{deployment.InstanceName}"
                    : deployment.PackageId)
                .Replace(" ", "-").ToLowerInvariant();

            subFolder = Path.Combine(subFolder, name);
            return subFolder;
        }
    }
}