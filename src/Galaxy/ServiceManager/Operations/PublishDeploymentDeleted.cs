using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.ServiceManager.Operations;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager
{
    public class PublishDeploymentDeletedEvent : IOperation
    {
        private readonly ObjectId _id;

        public PublishDeploymentDeletedEvent(Deployment deployment)
        {
            _id = deployment.Id;
        }

        public void Execute(DeploymentTaskContext context)
        {
            context.Publisher.Publish(new DeploymentDeletedEvent(_id));
        }
    }
}