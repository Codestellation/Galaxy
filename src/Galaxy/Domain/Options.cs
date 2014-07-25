using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class Options
    {
        public ObjectId Id { get; private set; }

        public string DeployFolder { get; set; }
        
        public string HostPackageId { get; set; }

        public string HostPackageFeedUri { get; set; }
    }
}