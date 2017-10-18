using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Events;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Operations
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
            context
                .Mediator
                .Send(new DeploymentDeletedEvent(_id)).Wait();
        }
    }
}