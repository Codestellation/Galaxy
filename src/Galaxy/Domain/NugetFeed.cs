using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class NugetFeed
    {
        public ObjectId Id { get; private set; }

        public string Name { get; set; }
        
        public string Uri { get; set; }

        public void Merge(NugetFeed other)
        {
            Name = other.Name;
            Uri = other.Uri;
        }
    }
}