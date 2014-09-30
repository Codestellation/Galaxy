using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeploymentTaskContext
    {
        public readonly TextWriter BuildLog;

        public DeploymentTaskContext(TextWriter buildLog)
        {
            BuildLog = buildLog;
        }
    }
}