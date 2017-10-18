using System.Collections.Generic;
using System.IO;
using Codestellation.Galaxy.Domain;
using MediatR;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeploymentTaskContext
    {
        public const string TaskName = "TaskName";

        public const string ServiceStatus = "ServiceStatus";
        public const string ForceStartService = "ForceStartService";
        public ObjectId DeploymentId { get; set; }
        public const string PublisherKey = "PublisherKey";
        public const string LogStream = "LogStream";

        public const string Config = "Config";

        public readonly TextWriter BuildLog;
        private readonly Dictionary<object, object> _data;

        public dynamic Parameters { get; set; }
        public ServiceFolders Folders { get; set; }
        public FileList KeepOnUpdate { get; set; }
        public string ServiceFileName { get; set; }
        public string ServiceName { get; set; }
        public string InstanceName { get; set; }

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
            if (_data.TryGetValue(key, out object temp))
            {
                value = (TValue)temp;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public IMediator Mediator => (IMediator)_data[PublisherKey];
        public PackageDetails PackageDetails { get; set; }
    }
}