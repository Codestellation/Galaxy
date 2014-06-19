using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Domain
{
    public class ServiceAppList
    {
        private readonly Dictionary<ObjectId, ServiceApp> _serviceApps;

        public ServiceAppList()
        {
            _serviceApps = new Dictionary<ObjectId, ServiceApp>();
        }

        public Dictionary<ObjectId, ServiceApp> ServiceApps
        {
            get { return _serviceApps; }
        } 

        public int ServiceAppsCount
        {
            get { return _serviceApps.Count; }
        }

        public void Add(ServiceApp item)
        {
            _serviceApps.Add(item.Id, item);
        }

        public ServiceApp this[ObjectId id]
        {
            get { return _serviceApps[id]; }
        }

        public void Remove(ObjectId id)
        {
            _serviceApps.Remove(id);
        }
    }
}
