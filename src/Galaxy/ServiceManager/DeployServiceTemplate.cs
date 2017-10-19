using System;
using System.Collections.Generic;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeployServiceTemplate : ITaskTemplate
    {
        public string Name { get; } = "Deploy Service";

        public IReadOnlyCollection<Type> Operations { get; } = new List<Type>
        {
            typeof(StopService),
            typeof(BackupService),
            typeof(ClearBinaries),
            typeof(EnsureFolders),
            typeof(InstallPackage),
            typeof(DeployHostConfig),
            typeof(GetConfigSample),
            typeof(DeployServiceConfig),
            typeof(StartService)
        };
    }
}