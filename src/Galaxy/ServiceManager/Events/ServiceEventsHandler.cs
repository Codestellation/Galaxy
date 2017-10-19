using Codestellation.Galaxy.Domain;
using MediatR;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class ServiceEventsHandler : IRequestHandler<DeploymentTaskRequest>
    {
        private readonly DeploymentTaskProcessor _taskProcessor;

        public ServiceEventsHandler(DeploymentTaskProcessor taskProcessor)
        {
            _taskProcessor = taskProcessor;
        }

        public void Handle(DeploymentTaskRequest message)
        {
            _taskProcessor.Enqueue(message);
        }
    }
}