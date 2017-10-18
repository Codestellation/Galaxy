using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.WebEnd.Controllers.Deployment;
using Codestellation.Galaxy.WebEnd.Models;
using Codestellation.Quarks.Collections;
using Codestellation.Quarks.IO;
using MediatR;
using Nancy.Security;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd
{
    public class FileModule : ModuleBase
    {
        public const string Path = DeploymentModule.Path + "/file";

        private readonly IMediator _mediator;
        private readonly TaskBuilder _taskBuilder;

        public FileModule(IMediator mediator, TaskBuilder taskBuilder)
            : base(Path)
        {
            _mediator = mediator;
            _taskBuilder = taskBuilder;

            this.RequiresAuthentication();

            Get["backup/{id}", true] = ShowBackups;
            Post["backup/{id}", true] = RestoreBackup;

            Get["/build-log/{id}", true] = GetBuildLogs;
            Get["/build-log/{id}/{filename}", true] = GetBuildLog;
        }

        private async Task<dynamic> ShowBackups(dynamic parameters, CancellationToken cancellationToken)
        {
            var id = new ObjectId(parameters.id);
            Deployment deployment = await GetDeployment(id).ConfigureAwait(false);

            var backupFolder = deployment.Folders.BackupFolder;

            var folders = Folder
                .EnumerateFolders((string)backupFolder)
                .SortDescending(x => x.CreationTime);

            return new BackupListModel(deployment.Id, folders);
        }

        private async Task<dynamic> RestoreBackup(dynamic parameters, CancellationToken cancellationToken)
        {
            var id = new ObjectId(parameters.id);
            Deployment deployment = await GetDeployment(id).ConfigureAwait(false);
            string name = Request.Query.name;

            var backupFolder = deployment.Folders.BackupFolder;
            var folder = Folder.Combine((string)backupFolder, name);
            var task = _taskBuilder.RestoreFromBackup(deployment, folder);

            task.Process();
            return string.Empty;
        }

        private async Task<dynamic> GetBuildLogs(dynamic parameters, CancellationToken cancellationToken)
        {
            var id = new ObjectId(parameters.id);
            Deployment deployment = await GetDeployment(id).ConfigureAwait(false);

            var logFolder = (string)deployment.Folders.DeployLogsFolder;

            var files = Folder
                .EnumerateFiles(logFolder)
                .SortDescending(x => x.LastWriteTime);

            return new BuildLogsModel(deployment.Id, files);
        }

        private async Task<dynamic> GetBuildLog(dynamic parameters, CancellationToken cancellationToken)
        {
            var id = new ObjectId(parameters.id);
            Deployment deployment = await GetDeployment(id).ConfigureAwait(false);
            string filename = parameters.filename;

            var fullPath = System.IO.Path.Combine((string)deployment.Folders.DeployLogsFolder, filename);

            return new FileResponse(fullPath);
        }

        private async Task<Deployment> GetDeployment(ObjectId id)
        {
            var request = new GetDeploymentRequest(id);
            var response = await _mediator.Send(request).ConfigureAwait(false);
            return response.Deployment;
        }
    }
}