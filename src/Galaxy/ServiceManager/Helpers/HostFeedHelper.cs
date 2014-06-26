using System.Configuration;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public static class HostFeedHelper
    {
        public static NugetFeed Create()
        {
            var hostPackageFeedUri = ConfigurationManager.AppSettings["hostPackageFeedUri"];
            var hostPackageName = ConfigurationManager.AppSettings["hostPackageName"];
            return new NugetFeed()
            {
                Name = hostPackageName,
                Uri = hostPackageFeedUri
            };
        }
    }
}
