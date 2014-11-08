using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain.Agents
{
    public class Agent
    {
        public ObjectId Id { get; set; }

        public AgentEndpoint Endpoint { get; set; }
        
    }

    public class AgentEndpoint
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string CreateUrl()
        {
            return string.Format("http://{0}:{1}", Host, Port);
        }
    }
}