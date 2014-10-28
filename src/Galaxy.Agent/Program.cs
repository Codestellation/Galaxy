using Codestellation.Galaxy.Host;

namespace Galaxy.Agent
{
    internal class Program
    {
        private static void Main()
        {
            Run.Service<AgentService>();
        }
    }
}