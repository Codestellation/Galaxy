using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement;
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

        public FileModule(IMediator mediator)
            : base(Path)
        {
            _mediator = mediator;

            this.RequiresAuthentication();

            Get["backup/{id}", true] = ShowBackups;

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