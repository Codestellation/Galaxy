using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.IO;
using Nancy.Security;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class FileModule : ModuleBase
    {
        public const string Path = DeploymentModule.Path + "/file";

        private readonly DeploymentBoard _deploymentBoard;
        private readonly TaskBuilder _taskBuilder;

        public FileModule(DeploymentBoard deploymentBoard, TaskBuilder taskBuilder)
            : base(Path)
        {
            _deploymentBoard = deploymentBoard;
            _taskBuilder = taskBuilder;

            this.RequiresAuthentication();

            Get["backup/{id}", true] = (parameters, token) => ProcessRequest(() => ShowBackups(parameters), token);
            Post["backup/{id}", true] = (parameters, token) => ProcessRequest(() => RestoreBackup(parameters), token);
        }

        private object ShowBackups(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);

            var backupFolder = deployment.GetBackupFolder();

            var folders = Folder
                .EnumerateFolders(backupFolder)
                .SortDescending(x => x.CreationTime);

            return new BackupListModel(deployment.Id, folders);
        }

        private object RestoreBackup(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _deploymentBoard.GetDeployment(id);
            string name = Request.Query.name;

            var backupFolder = deployment.GetBackupFolder();
            var folder = Folder.Combine(backupFolder, name);

            var task = _taskBuilder.RestoreFromBackup(deployment, folder);

            task.Process();

            return string.Empty;
        }

        private Deployment GetDeployment(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _deploymentBoard.GetDeployment(id);
            return deployment;
        }
    }
}