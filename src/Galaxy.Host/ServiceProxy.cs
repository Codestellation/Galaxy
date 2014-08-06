using System;
using System.Reflection;
using NLog;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly MethodInfo _startMethod;
        private readonly MethodInfo _stopMethod;
        private readonly object _instance;

        public ServiceProxy(Type serviceType, ServiceConfig config)
        {
            _instance = Activator.CreateInstance(serviceType);
            _startMethod = GetMethodOrThrow(config.StartMethod, serviceType);
            _stopMethod = GetMethodOrThrow(config.StopMethod, serviceType);
        }

        private static MethodInfo GetMethodOrThrow(string methodName, Type serviceType)
        {
            var result = serviceType.GetMethod(methodName);
            if (result != null)
            {
                return result;
            }
            var message = string.Format("Method '{0}' of type '{1}' not found", methodName, serviceType);
            Logger.Fatal(message);
            throw new InvalidOperationException(message);
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