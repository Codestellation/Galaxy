using System.IO;
using Castle.MicroKernel;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.WebEnd.Controllers.DeploymentManagement;
using Codestellation.Quarks.DateAndTime;
using Codestellation.Quarks.IO;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TaskBuilder
    {
        private readonly IMediator _mediator;
        private readonly IKernel _kernel;
        private readonly TemplateService _service;

        public TaskBuilder(IMediator mediator, IKernel kernel, TemplateService service)
        {
            _mediator = mediator;
            _kernel = kernel;
            _service = service;
        }

        public DeploymentTask Build(DeploymentTaskRequest request)
        {
            var (deployment, feed) = GetDeployment(request.DeploymentId);
            return CreateDeployTask(request.TaskName, deployment, feed, request.Parameters);
        }

        private (Deployment deployment, NugetFeed feed) GetDeployment(ObjectId id)
        {
            var request = new GetDeploymentRequest(id);
            var response = _mediator.Send(request).Result;
            return (response.Deployment, response.Feed);
        }

        private DeploymentTask CreateDeployTask(string name, Deployment deployment, NugetFeed deploymentFeed, object parameters = null, Stream logStream = null)
        {
            var actualLogStream = logStream ?? BuildDefaultLogStream(name, deployment);
            var streamWriter = new StreamWriter(actualLogStream);

            var context = new DeploymentTaskContext(streamWriter)
            {
                TaskName = name,
                Parameters = parameters ?? new object(),
                DeploymentId = deployment.Id,
                Folders = deployment.Folders,
                ServiceFileName = $"{deployment.PackageId}.exe",
                InstanceName = deployment.InstanceName,
                ServiceName = string.IsNullOrWhiteSpace(deployment.InstanceName)
                    ? deployment.PackageId
                    : $"{deployment.PackageId}${deployment.InstanceName}",
                PackageDetails = new PackageDetails(deployment.PackageId, deploymentFeed.Uri, deployment.PackageVersion),
                Mediator = _mediator,
                LogStream = actualLogStream,
                Config = deployment.Config
            };

            var template = _service.GetTemplate(name);
            var task = new DeploymentTask(context);

            foreach (var operationType in template.Operations)
            {
                var operation = (IOperation)_kernel.Resolve(operationType);
                task.Add(operation);
            }
            return task;
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
    }
}