using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain.Agents;
using Codestellation.Quarks.Collections;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class AgentListModel
    {
        private AgentItemModel[] _agents;

        public AgentListModel(IReadOnlyCollection<Agent> agents)
        {
            _agents = agents.ConvertToArray(x => new AgentItemModel(x), agents.Count);
        }

        public AgentItemModel[] Agents
        {
            get { return _agents ?? new AgentItemModel[0]; }
            set { _agents = value; }
        }
    }

    public class AgentItemModel
    {
        public AgentItemModel(Agent agent)
        {
            Id = agent.Id;
            Host = agent.Endpoint.Host;
            Port = agent.Endpoint.Port;
        }

        public ObjectId Id { get; set; }
        
        public string Host { get; set; }

        public int Port { get; set; }

        public bool Connected { get; set; }
    }
}