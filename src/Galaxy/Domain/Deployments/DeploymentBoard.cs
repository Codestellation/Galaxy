using System;
using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.Host;
using Codestellation.Galaxy.Infrastructure;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain.Deployments
{
    public class DeploymentBoard
    {
        private readonly Options _options;
        private readonly HostConfig _config;
        private readonly Dictionary<ObjectId, Deployment> _deployments;
        private readonly Collection _deploymentCollection;

        public DeploymentBoard(Repository repository, Options options, HostConfig config)
        {
            _options = options;
            _config = config;
            _deploymentCollection = repository.GetCollection<Deployment>();
            _deployments = new Dictionary<ObjectId, Deployment>();

            var collection = repository.GetCollection<Deployment>();
            using (var query = collection.CreateQuery<Deployment>())
            using (var cursor = query.Execute())
            {
                foreach (var deployment in cursor)
                {
                    _deployments.Add(deployment.Id, deployment);
                }
            }
        }

        public IReadOnlyCollection<Deployment> Deployments
        {
            get { return new List<Deployment>(_deployments.Values); }
        }

        public void AddDeployment(Deployment deployment)
        {
            if (!deployment.Id.IsEmpty)
            {
                throw new ArgumentException("Deployment must be new deployment", "deployment");
            }
            SaveDeployment(deployment);

            //to make folders
            FillServiceFolders(deployment);

            SaveDeployment(deployment);

            _deployments.Add(deployment.Id, deployment);
        }

        public Deployment GetDeployment(ObjectId id)
        {
            return _deployments[id];
        }

        public bool TryGetDeployment(ObjectId id, out Deployment deployment)
        {
            return _deployments.TryGetValue(id, out deployment);
        }

        public void RemoveDeployment(ObjectId id)
        {
            _deploymentCollection.Delete(id);
            _deployments.Remove(id);
        }

        private void FillServiceFolders(Deployment deployment)
        {
            var serviceFolders = deployment.Folders;

            var subfolder = BuildSubfolder(deployment);

            var deployFolderPath = Path.Combine(_options.GetDeployFolder(), subfolder);
            serviceFolders.DeployFolder = (FullPath)deployFolderPath;

            BuildFolders(deployment);
        }

        public void SaveDeployment(Deployment deployment)
        {
            using (var tx = _deploymentCollection.BeginTransaction())
            {
                _deploymentCollection.Save(deployment, false);
                tx.Commit();
            }
        }

        private void BuildFolders(Deployment deployment)
        {
            string subFolder = BuildSubfolder(deployment);
            deployment.Folders.DeployLogsFolder = (FullPath)Path.Combine(_config.Logs.FullName, "deploy-logs", subFolder);
            deployment.Folders.BackupFolder = (FullPath)Path.Combine(_config.Data.FullName, "backups", subFolder);
            deployment.Folders.Logs = (FullPath)Path.Combine(_options.FolderOptions.Logs, subFolder);
            deployment.Folders.Configs = (FullPath)Path.Combine(_options.FolderOptions.Configs, subFolder);
            deployment.Folders.Data = (FullPath)Path.Combine(_options.FolderOptions.Data, subFolder);
        }

        private static string BuildSubfolder(Deployment deployment)
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