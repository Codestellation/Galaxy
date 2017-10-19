using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskRequest : IRequest
    {
        public ObjectId DeploymentId { get; }
        public string TaskName { get; }
        public object Parameters { get; }

        public DeploymentTaskRequest(ObjectId deploymentId, string taskName, object parameters = null)
        {
            DeploymentId = deploymentId;
            TaskName = taskName;
            Parameters = parameters ?? new object();
        }
    }
}