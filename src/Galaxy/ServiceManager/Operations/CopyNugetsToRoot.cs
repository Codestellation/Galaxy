using System.Linq;
using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class CopyNugetsToRoot : OperationBase
    {
        private const string LibFolder = "lib";
        private readonly string _hostPackageName;

        public CopyNugetsToRoot(string basePath, Deployment deployment) :
            base(basePath, deployment)
        {
            _hostPackageName = ConfigurationManager.AppSettings["hostPackageName"] ?? "Codestellation.Galaxy.Host";
        }


        public override void Execute(TextWriter buildLog)
        {
            var packageFolders = Directory.EnumerateDirectories(ServiceFolder).ToArray();

            var packageDotNetVersions = new Dictionary<string, Version>();
            var hostPackageDotNetVersion = new Version();
            var packagesDotNetVersionMax = new Version();

            bool isHostPackagePresent = false;

            foreach (var packagePath in packageFolders)
            {
                var packageDotNetVersion = GetNugetDotNetVersion(packagePath);

                if (packagePath.Contains(_hostPackageName))
                {
                    hostPackageDotNetVersion = packageDotNetVersion;
                    isHostPackagePresent = true;
                }
                else
                {
                    packageDotNetVersions.Add(packagePath, packageDotNetVersion);
                    
                    if (packagesDotNetVersionMax < packageDotNetVersion)
                    {
                        packagesDotNetVersionMax = packageDotNetVersion;
                    }
                }
            }

            if (isHostPackagePresent && packagesDotNetVersionMax > hostPackageDotNetVersion)
            {
                throw new InvalidOperationException("Found incompatible package with host service application, reason: .NET framework version");
            }

            Action copyHost = null;

            foreach (var packagePath in packageFolders)
            {
                if (packagePath.Contains(_hostPackageName))
                {
                    // unpacking host app package should be after all other packages
                    copyHost = () => UnpackPackage(packagePath, ServiceFolder, hostPackageDotNetVersion);
                }
                else
                {
                    UnpackPackage(packagePath, ServiceFolder, packageDotNetVersions[packagePath]);
                }
            }

            if (copyHost != null)
            {
                copyHost();
            }

            Clean(packageFolders);

            if (!isHostPackagePresent)
            {
                throw new InvalidOperationException("Host package wasn't found");
            }
        }
        private Version GetNugetDotNetVersion(string installedNugetPath)
        {
            Version result = new Version();

            var libFolderPath = Path.Combine(installedNugetPath, LibFolder);
            if (Directory.Exists(libFolderPath))
            {
                foreach (var pair in DotNetVersionHelper.dotNetNugetFolders)
                {
                    var libFolderDotNetVersionedPath = Path.Combine(libFolderPath, pair.Value);

                    if (!Directory.Exists(libFolderDotNetVersionedPath))
                    {
                        continue;
                    }

                    if (result < pair.Key)
                    {
                        result = pair.Key;
                    }
                }
            }

            // temporary, if no lib/ folder
            if (result.Equals(new Version()))
            {
                result = new Version(4, 5);
            }

            return result;
        }
        private void UnpackPackage(string packagePath, string serviceTargetPath, Version targetDotNetVersion)
        {
            var binariesPath = DetectBinariesPath(packagePath, targetDotNetVersion);
            binariesPath.CopyIncludeSubfoldersTo(serviceTargetPath);
        }

        private string DetectBinariesPath(string packagePath, Version targetDotNetVersion)
        {
            var libFolderPath = Path.Combine(packagePath, LibFolder);
            var libFolderDotNetVersionedPath = Path.Combine(libFolderPath, DotNetVersionHelper.dotNetNugetFolders[targetDotNetVersion]);

            if (Directory.Exists(libFolderDotNetVersionedPath))
            {
                return libFolderDotNetVersionedPath;
            }

            if (Directory.Exists(libFolderPath))
            {
                return libFolderPath;
            }

            return packagePath;
        }

        private void Clean(IEnumerable<String> packageFolders)
        {
            foreach (var packagePath in packageFolders)
            {
                Directory.Delete(packagePath, true);
            }
        }
    }
}