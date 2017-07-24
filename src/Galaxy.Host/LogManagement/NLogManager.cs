using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Codestellation.Quarks.IO;
using Topshelf.HostConfigurators;

namespace Codestellation.Galaxy.Host.LogManagement
{
    internal class NLogManager
    {
        internal static void TryInitialize(HostConfigurator hostConfigurator, Assembly assembly, DirectoryInfo logs)
        {
            if (NlogDetectedIn(assembly))
            {
                ConfigureNLog(hostConfigurator, logs);
            }
        }

        private static void ConfigureNLog(HostConfigurator hostConfigurator, DirectoryInfo logs)
        {
            var logManagerType = Type.GetType("NLog.LogManager, NLog");

            if (logManagerType == null)
            {
                return;
            }

            ConfigureNLog(logs, logManagerType);
            ConfigureHostLogger(hostConfigurator, logManagerType);
        }

        private static void ConfigureNLog(DirectoryInfo logs, Type logManagerType)
        {
            string nlogFile = Folder.Combine(logs.FullName, "nlog.config");

            if (!File.Exists(nlogFile))
            {
                throw new FileNotFoundException($"Could not find ${nlogFile}", nlogFile);
            }

            //LogManager.Configuration = new XmlLoggingConfiguration(path);

            var configurationProperty = Expression.Property(null, logManagerType, "Configuration");

            var xmlConfigurationType = Type.GetType("NLog.Config.XmlLoggingConfiguration, NLog");

            var constructor = xmlConfigurationType
                .GetConstructors()
                .Single(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(string));

            var nlogConfigPath = Expression.Constant(nlogFile);
            var xmlConfiguration = Expression.New(constructor, nlogConfigPath);

            var assingProperty = Expression.Assign(configurationProperty, xmlConfiguration);

            var configureNLog = Expression.Lambda<Action>(assingProperty).Compile();
            configureNLog();
        }

        private static void ConfigureHostLogger(HostConfigurator hostConfigurator, Type logManagerType)
        {
            var topShelfNLogType = Type.GetType("Topshelf.NLogConfiguratorExtensions,Topshelf.NLog");

            if (topShelfNLogType == null)
            {
                return;
            }

            //Expressions below emulate this code
            //x.UseNLog(LogManager.GetLogger(typeof(Run).FullName).Factory);

            var hostConfigExpression = Expression.Constant(hostConfigurator);
            var loggerNameExpression = Expression.Constant(typeof(Run).FullName);
            var getLoggerCall = Expression.Call(logManagerType, "GetLogger", null, loggerNameExpression);
            var getFactoryCall = Expression.PropertyOrField(getLoggerCall, "Factory");
            var callUseNLog = Expression.Call(topShelfNLogType, "UseNLog", null, hostConfigExpression, getFactoryCall);

            var configureHostLogger = Expression.Lambda<Action>(callUseNLog).Compile();

            configureHostLogger();
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