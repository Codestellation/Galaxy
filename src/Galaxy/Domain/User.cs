using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class User
    {
        public ObjectId Id { get; internal set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string DispalyName { get; set; }

        public bool IsAdmin { get; set; }
    }
}