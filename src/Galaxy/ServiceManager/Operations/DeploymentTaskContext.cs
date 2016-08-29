using System.Collections.Generic;
using System.IO;
using Codestellation.Emisstar;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeploymentTaskContext
    {
        public const string TaskName = "TaskName";

        public const string ServiceStatus = "ServiceStatus";
        public const string ForceStartService = "ForceStartService";
        public const string DeploymentId = "DeploymentId";
        public const string PublisherKey = "PublisherKey";
        public const string LogStream = "LogStream";

        public const string Config = "Config";
        public const string Folders = "Folders";

        public readonly TextWriter BuildLog;
        private readonly Dictionary<object, object> _data;

        public DeploymentTaskContext(TextWriter buildLog)
        {
            _data = new Dictionary<object, object>();
            BuildLog = buildLog;
        }

        public DeploymentTaskContext SetValue<TValue>(object key, TValue value)
        {
            _data.Add(key, value);
            return this;
        }

        public TValue GetValue<TValue>(object key)
        {
            return (TValue)_data[key];
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

        public IPublisher Publisher
        {
            get { return (IPublisher)_data[PublisherKey]; }
        }
    }
}