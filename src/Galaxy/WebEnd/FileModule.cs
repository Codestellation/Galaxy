using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Galaxy.WebEnd.Models.Deployment;
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

            Get["/{id}", true] = (parameters, token) => ProcessRequest(() => GetFiles(parameters), token);
            Get["/{id}/{filename}", true] = (parameters, token) => ProcessRequest(() => GetFile(parameters), token);

            Get["backup/{id}", true] = (parameters, token) => ProcessRequest(() => ShowBackups(parameters), token);
            Post["backup/{id}", true] = (parameters, token) => ProcessRequest(() => RestoreBackup(parameters), token);

            Post["/{id}", true] = (parameters, token) => ProcessRequest(() => PostFiles(parameters), token);
            Post["delete/{id}", true] = (parameters, token) => ProcessRequest(() => DeleteFile(parameters), token);
        }

        private object GetFiles(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);

            var filesFolder = deployment.GetFilesFolder();

            var filesDirectory = Folder.EnsureExists(filesFolder);

            var files = Folder
                .EnumerateFilesRecursive(filesFolder)
                .SortAscending(x => x.Name);

            return new DeploymentFilesModel(deployment.Id, filesDirectory, files);
        }

        private object GetFile(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);
            string filename = parameters.filename;
            var filesFolder = deployment.GetFilesFolder();
            var fullpath = Folder.Combine(filesFolder, filename);

            return new FileResponse(fullpath);
        }

        private object PostFiles(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);
            var file = Request.Files.First();
            var filename = file.Name;

            string folderName = Request.Form.PutTo;

            folderName = string.IsNullOrWhiteSpace(folderName) ? string.Empty : folderName;

            var folderPath = Folder.Combine(deployment.GetFilesFolder(), folderName);
            Folder.EnsureExists(folderPath);

            var fullPath = Folder.Combine(folderPath, filename);

            using (var stream = File.OpenWrite(fullPath))
            {
                file.Value.CopyTo(stream);
            }

            return GetFiles(parameters);
        }

        private object DeleteFile(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);
            string filename = Request.Query.file;
            var filesFolder = deployment.GetFilesFolder();
            var fullpath = Folder.Combine(filesFolder, filename);

            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }

            return string.Empty;
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