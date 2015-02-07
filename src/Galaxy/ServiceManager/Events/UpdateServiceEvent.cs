using System;
using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class UpdateServiceEvent 
    {
        public readonly ObjectId DeploymentId;
        public readonly Version Version;

        public UpdateServiceEvent(ObjectId deploymentId, Version version)
        {
            DeploymentId = deploymentId;
            Version = version;
        }
    }
}