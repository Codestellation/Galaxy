using Codestellation.Galaxy.Domain;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class FeedModel
    {
        public bool InUse { get; set; }

        public FeedModel()
        {

        }
        public FeedModel(NugetFeed feed, bool inUse)
        {
            InUse = inUse;
            Id = feed.Id;
            Name = feed.Name;
            Uri = feed.Uri;
        }

        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public NugetFeed ToFeed()
        {
            return new NugetFeed { Name = Name, Uri = Uri };
        }
    }
}