using System;
using System.Reflection;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        private readonly MethodInfo _startMethod;
        private readonly MethodInfo _stopMethod;
        private readonly object _instance;

        public ServiceProxy(ServiceConfig config)
        {
            var serviceType = Type.GetType(config.AssemblyQualifiedType);

            _instance = Activator.CreateInstance(serviceType);
            _startMethod = serviceType.GetMethod(config.StartMethod);
            _stopMethod = serviceType.GetMethod(config.StopMethod);
        }

        public void Start()
        {
            _startMethod.Invoke(_instance, new object[0]);
        }

        public void Stop()
        {
            _stopMethod.Invoke(_instance, new object[0]);
        }
    }
}