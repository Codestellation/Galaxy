using Codestellation.Galaxy.ServiceManager.Helpers;
using System;
using System.IO;
using System.Linq;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ConfigurePlatform : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;
        private readonly string _assemblyQualifiedType;

        public ConfigurePlatform(string serviceFolder, string hostFileName, string assemblyQualifiedType)
        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
            _assemblyQualifiedType = assemblyQualifiedType;
        }

        public void Execute(TextWriter buildLog)
        {
            var serviceLib = GetServiceLibName();
            var libPath = Path.Combine(_serviceFolder, serviceLib);

            if (!File.Exists(libPath))
            {
                throw new FileNotFoundException("Can't find library.", libPath);
            }

            var platform = PlatformDetector.GetPlatform(libPath);

            PlatformDetector.ApplyPlatformToHost(platform, Path.Combine(_serviceFolder, _hostFileName));
        }

        private string GetServiceLibName()
        {
            if (string.IsNullOrEmpty(_assemblyQualifiedType))
            {
                throw new ArgumentException("Can't get service library name from AssemblyQualifiedType (null or empty)");
            }

            var assemblyNameParts = _assemblyQualifiedType.Split(',');

            if (assemblyNameParts.Count() != 2)
            {
                throw new ArgumentException("Can't get service library name from AssemblyQualifiedType (format error)");
            }

            var assemblyName = assemblyNameParts.ElementAt(1).TrimStart(' ');

            return assemblyName + ".dll";
        }
    }
}