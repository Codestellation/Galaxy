using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.Collections.Generic;
using System;
using System.Configuration;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class CopyNugetsToRoot : OperationBase
    {
        private const string LibFolder = "lib";
        private readonly string _hostPackageName;

        public CopyNugetsToRoot(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {
            _hostPackageName = ConfigurationManager.AppSettings["hostPackageName"];
        }

        public override void Execute()
        {
            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);

            var packageFolders = Directory.EnumerateDirectories(serviceTargetPath).ToArray();

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
                    copyHost = () => UnpackPackage(packagePath, serviceTargetPath, hostPackageDotNetVersion);
                }
                else
                    UnpackPackage(packagePath, serviceTargetPath, packageDotNetVersions[packagePath]);
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
            var libFolderPath = Path.Combine(packagePath, LibFolder);
            if (Directory.Exists(libFolderPath))
            {
                var libFolderDotNetVersionedPath = Path.Combine(libFolderPath, DotNetVersionHelper.dotNetNugetFolders[targetDotNetVersion]);

                if (Directory.Exists(libFolderDotNetVersionedPath))
                {
                    // copy from "{package name}/lib/{dotnetversionfolder}/" to "../" dir
                    libFolderDotNetVersionedPath.CopyIncludeSubfoldersTo(serviceTargetPath);
                }
                else
                {
                    // copy from "{package name}/lib/" to "../" dir
                    libFolderPath.CopyIncludeSubfoldersTo(serviceTargetPath);
                }
            }
            else
            {
                // copy from "{package name}/" to "../" dir
                packagePath.CopyIncludeSubfoldersTo(serviceTargetPath);
            }
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