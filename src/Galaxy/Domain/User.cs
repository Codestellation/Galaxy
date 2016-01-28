using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class User
    {
        private string _login;
        public ObjectId Id { get; internal set; }

        public string Login
        {
            get { return _login; }
            set { _login = string.IsNullOrWhiteSpace(value) ? value : value.ToLowerInvariant(); }
        }

        public bool IsAdmin { get; set; }

        public bool IsDomain { get; set; }
    }
}