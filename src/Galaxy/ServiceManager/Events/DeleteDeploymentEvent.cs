using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class DeleteDeploymentEvent 
    {
        public readonly ObjectId DeploymentId;

        public DeleteDeploymentEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}