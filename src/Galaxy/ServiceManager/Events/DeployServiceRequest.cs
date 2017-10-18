using System;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeployServiceRequest : IRequest
    {
        public readonly ObjectId DeploymentId;
        public readonly Version Version;

        public DeployServiceRequest(ObjectId deploymentId, Version version)
        {
            DeploymentId = deploymentId;
            Version = version;
        }
    }
}