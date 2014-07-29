using Codestellation.Galaxy.Infrastructure;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class Options
    {
        public ObjectId Id { get; private set; }

        public string RootDeployFolder { get; set; }
        
        public string HostPackageId { get; set; }

        public string HostPackageFeedUri { get; set; }

        public string GetDeployFolder()
        {
            if (string.IsNullOrWhiteSpace(RootDeployFolder))
            {
                return Folder.BasePath;
            }

            return RootDeployFolder;
        }

        public string GetHostPackageId()
        {
            if (string.IsNullOrWhiteSpace(HostPackageId))
            {
                return "Codestellation.Galaxy.Host";
            }
            return HostPackageId;
        }

        public string GetHostFileName()
        {
            return "Codestellation.Galaxy.Host.exe";
        }
    }
}