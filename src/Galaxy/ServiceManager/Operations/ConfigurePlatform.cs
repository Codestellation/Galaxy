using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System;
using System.IO;
using System.Linq;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ConfigurePlatform : WinServiceOperation
    {
        public ConfigurePlatform(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {
        }

        public override void Execute(StringBuilder buildLog)
        {
            var serviceLib = GetServiceLibName();
            var libPath = Path.Combine(ServiceFolder, serviceLib);

            if (!File.Exists(libPath))
            {
                throw new FileNotFoundException("Can't find library.", libPath);
            }

            var platform = PlatformDetector.GetPlatform(libPath);

            PlatformDetector.ApplyPlatformToHost(platform, Path.Combine(ServiceFolder, ServiceHostFileName));
        }

        private string GetServiceLibName()
        {
            if (string.IsNullOrEmpty(Deployment.AssemblyQualifiedType))
            {
                throw new ArgumentException("Can't get service library name from AssemblyQualifiedType (null or empty)");
            }

            var assemblyNameParts = Deployment.AssemblyQualifiedType.Split(',');

            if (assemblyNameParts.Count() != 2)
            {
                throw new ArgumentException("Can't get service library name from AssemblyQualifiedType (format error)");
            }

            var assemblyName = assemblyNameParts.ElementAt(1).TrimStart(' ');

            return assemblyName + ".dll";
        }
    }
}