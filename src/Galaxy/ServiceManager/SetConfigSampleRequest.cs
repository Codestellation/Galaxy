using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class SetConfigSampleRequest : IRequest
    {
        public ObjectId DeploymentId { get; }
        public string Sample { get; }

        public SetConfigSampleRequest(ObjectId deploymentId, string sample)
        {
            DeploymentId = deploymentId;
            Sample = sample;
        }
    }
}