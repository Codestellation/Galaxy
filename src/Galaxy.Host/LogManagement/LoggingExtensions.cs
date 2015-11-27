using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Topshelf.HostConfigurators;

namespace Codestellation.Galaxy.Host.LogManagement
{
    internal static class LoggingExtensions
    {
        public static void InitializeLoggers(this HostConfigurator hostConfigurator, Assembly assembly)
        {
            if (NlogDetectedIn(assembly))
            {
                ConfigureNLog(hostConfigurator);
            }
        }

        private static void ConfigureNLog(HostConfigurator hostConfigurator)
        {
            var logManagerType = Type.GetType("NLog.LogManager, NLog");

            var topShelfNLogType = Type.GetType("Topshelf.NLogConfiguratorExtensions,Topshelf.NLog");

            if (logManagerType == null || topShelfNLogType == null)
            {
                return;
            }

            //Topshelf.NLogConfiguratorExtensions.UseNLog(this HostConfigurator configurator, LogFactory factory)

            //Expressions below emulate this code
            //x.UseNLog(LogManager.GetLogger(typeof(Run).FullName).Factory);
            var hostConfigExpression = Expression.Constant(hostConfigurator);
            var loggerNameExpression = Expression.Constant(typeof(Run).FullName);
            var getLoggerCall = Expression.Call(logManagerType, "GetLogger", null, loggerNameExpression);
            var getFactoryCall = Expression.PropertyOrField(getLoggerCall, "Factory");
            var callUseNLog = Expression.Call(topShelfNLogType, "UseNLog", null, hostConfigExpression, getFactoryCall);

            var configLoggerAction = Expression.Lambda<Action>(callUseNLog).Compile();

            configLoggerAction();
        }

        private static bool NlogDetectedIn(Assembly assembly)
        {
            var nlogDetected = assembly
                .GetReferencedAssemblies()
                .Any(x => string.Equals(x.Name, "NLog", StringComparison.OrdinalIgnoreCase));
            return nlogDetected;
        }
    }
}