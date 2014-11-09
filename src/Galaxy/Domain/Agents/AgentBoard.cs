using System.Collections.Generic;
using Codestellation.Galaxy.Infrastructure;
using Nejdb;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain.Agents
{
    public class AgentBoard
    {
        private readonly Collection _agents;
        private readonly List<Agent> _cache;

        public AgentBoard(Repository repository)
        {
            _agents = repository.GetCollection<Agent>();

            var loadedAgents = _agents.PerformQuery<Agent>();
            
            _cache = new List<Agent>(loadedAgents);
        }

        public IReadOnlyCollection<Agent> Agents
        {
            get { return _cache; }
        }

        public Agent Create(AgentEndpoint endpoint)
        {
            var result = new Agent { Endpoint = endpoint };

            using (var transaction = _agents.BeginTransaction())
            {
                _agents.Save(result, false);
                transaction.Commit();
            }

            _cache.Add(result);
            return result;
        }

        public Agent GetAgent(ObjectId id)
        {
            return _cache.Find(x => x.Id.Equals(id));
        }
    }
}