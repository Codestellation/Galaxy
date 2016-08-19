using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain.Deployments;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class ConfigSampleReceivedHandler : IHandler<ConfigSampleReceived>
    {
        private readonly DeploymentBoard _board;

        public ConfigSampleReceivedHandler(DeploymentBoard board)
        {
            _board = board;
        }

        public void Handle(ConfigSampleReceived message)
        {
            var deployment = _board.GetDeployment(message.DeploymentId);
            deployment.ConfigSample = message.Sample;
            _board.SaveDeployment(deployment);
        }
    }
}