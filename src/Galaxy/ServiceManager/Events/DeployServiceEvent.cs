using System;
using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeployServiceEvent : IMainRequest
    {
        public readonly ObjectId DeploymentId;
        public readonly Version Version;

        public DeployServiceEvent(ObjectId deploymentId, Version version)
        {
            DeploymentId = deploymentId;
            Version = version;
        }
    }
}