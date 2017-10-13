using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class StartServiceEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;

        public StartServiceEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}