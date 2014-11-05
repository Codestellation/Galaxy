using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class AgentListModel
    {
        private AgentItemModel[] _agents;

        public AgentItemModel[] Agents
        {
            get { return _agents ?? new AgentItemModel[0]; }
            set { _agents = value; }
        }
    }

    public class AgentItemModel
    {
        public ObjectId Id { get; set; }
        
        public string Host { get; set; }

        public int Port { get; set; }

        public bool Connected { get; set; }
    }
}