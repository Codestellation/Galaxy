using System.Collections.Generic;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class DeploymentList
    {
        private readonly Dictionary<ObjectId, Deployment> _deployments;

        public DeploymentList()
        {
            _deployments = new Dictionary<ObjectId, Deployment>();
        }

        public Dictionary<ObjectId, Deployment> Deployments
        {
            get { return _deployments; }
        } 

        public int DeploymentCount
        {
            get { return _deployments.Count; }
        }

        public void Add(Deployment item)
        {
            _deployments.Add(item.Id, item);
        }

        public Deployment this[ObjectId id]
        {
            get { return _deployments[id]; }
        }

        public void Remove(ObjectId id)
        {
            _deployments.Remove(id);
        }
    }
}
