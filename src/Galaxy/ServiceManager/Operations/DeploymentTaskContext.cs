using System.Collections.Generic;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeploymentTaskContext
    {
        public const string ServiceStatus = "ServiceStatus";
        public const string ForceStartService = "ForceStartService";

        public readonly TextWriter BuildLog;
        private Dictionary<object, object> _data;

        public DeploymentTaskContext(TextWriter buildLog)
        {
            _data = new Dictionary<object, object>();
            BuildLog = buildLog;
        }

        public void SetValue<TValue>(object key, TValue value)
        {
            _data.Add(key, value);
        }

        public TValue GetValue<TValue>(object key)
        {
            return (TValue) _data[key];
        }

        public bool TryGetValue<TValue>(object key, out TValue value)
        {
            object temp;
            if (_data.TryGetValue(key, out temp))
            {
                value = (TValue)temp;    
                return true;
            }

            value = default(TValue);
            return false;
        }
    }
}