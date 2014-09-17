using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
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

        private readonly DashBoard _dashBoard;

        public FileModule(DashBoard dashBoard)
            : base(Path)
        {
            _dashBoard = dashBoard;

            this.RequiresAuthentication();

            Get["/{id}", true] = (parameters, token) => ProcessRequest(() => GetFiles(parameters), token);
            Get["/{id}/{filename}", true] = (parameters, token) => ProcessRequest(() => GetFile(parameters), token);
            Post["/{id}", true] = (parameters, token) => ProcessRequest(() => PostFiles(parameters), token);
            Post["delete/{id}", true] = (parameters, token) => ProcessRequest(() => DeleteFile(parameters), token);
        }

        private object GetFiles(dynamic parameters)
        {
            Deployment deployment = GetDeployment(parameters);

            var filesFolder = deployment.GetFilesFolder();

            Folder.EnsureExists(filesFolder);

            var files = Folder
                .EnumerateFiles(filesFolder)
                .SortAscending(x => x.Name);

            return new DeploymentFilesModel(deployment.Id, files);
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

            var fullPath = Folder.Combine(deployment.GetFilesFolder(), filename);

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

        private Deployment GetDeployment(dynamic parameters)
        {
            var id = new ObjectId(parameters.id);
            var deployment = _dashBoard.GetDeployment(id);
            return deployment;
        }
    }
}