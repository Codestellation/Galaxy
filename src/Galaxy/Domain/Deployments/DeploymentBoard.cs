using System;
using System.Collections.Generic;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Quarks.IO;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain.Deployments
{
    public class DeploymentBoard
    {
        private readonly Options _options;
        private readonly Dictionary<ObjectId, Deployment> _deployments;
        private readonly Collection _deploymentCollection;

        public DeploymentBoard(Repository repository, Options options)
        {
            _options = options;
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
            var serviceFolders = deployment.ServiceFolders;

            var fullPath = string.IsNullOrWhiteSpace(deployment.InstanceName)
                ? Folder.Combine(_options.GetDeployFolder(), deployment.PackageId)
                : Folder.Combine(_options.GetDeployFolder(), string.Format("{0}-{1}", deployment.PackageId, deployment.InstanceName));
            var specialFolder = new SpecialFolder(SpecialFolderDictionary.DeployFolder, fullPath);
            serviceFolders.Add(specialFolder);


            BuildServiceFolder(deployment, SpecialFolderDictionary.DeployLogsFolder, "BuildLogs");
            BuildServiceFolder(deployment, SpecialFolderDictionary.FileOverrides, "FileOverrides");
            BuildServiceFolder(deployment, SpecialFolderDictionary.BackupFolder, "Backups");
        }

        private void BuildServiceFolder(Deployment deployment, string purpose, string subfolder)
        {
            var fullPath = Folder.Combine(Folder.BasePath, deployment.Id.ToString(), subfolder);
            var folder = new SpecialFolder(purpose, fullPath);
            deployment.ServiceFolders.Add(folder);
        }

        public void SaveDeployment(Deployment deployment)
        {
            using (var tx = _deploymentCollection.BeginTransaction())
            {
                _deploymentCollection.Save(deployment, false);
                tx.Commit();
            }
        }
    }
}