using Codestellation.Galaxy.Host;

namespace Codestellation.Galaxy
{
    internal static class Program
    {
        private static int Main()
        {
            return Run.Service<Service>();
        }
    }
}