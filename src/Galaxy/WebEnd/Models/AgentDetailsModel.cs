using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class AgentDetailsModel
    {
        public ObjectId Id { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}