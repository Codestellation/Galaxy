using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class User
    {
        public ObjectId Id { get; internal set; }

        public string Login { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsDomain { get; set; }
    }
}