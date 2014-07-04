using System.Configuration;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public static class HostDeployHelper
    {
        public static Deployment CreateDeployment(Deployment deployment)
        {
            var hostPackageName = ConfigurationManager.AppSettings["hostPackageName"];
            return new Deployment()
            {
                DisplayName = deployment.DisplayName,
                PackageName = hostPackageName,
                PackageVersion = new System.Version(1,0)
            };
        }

        public static NugetFeed CreateFeed()
        {
            var hostPackageFeedUri = ConfigurationManager.AppSettings["hostPackageFeedUri"];
            return new NugetFeed()
            {
                Name = "host_nuget_feed",
                Uri = hostPackageFeedUri
            };
        }
    }
}
