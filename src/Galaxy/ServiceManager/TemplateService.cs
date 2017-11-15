using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public class TemplateService
    {
        private readonly Dictionary<string, ITaskTemplate> _templates;

        public TemplateService()
        {
            _templates = BuildTemplates().ToDictionary(x => x.Name, x => x);
        }

        public ITaskTemplate GetTemplate(string name)
        {
            if (_templates.TryGetValue(name, out ITaskTemplate template))
            {
                return template;
            }
            throw new InvalidOperationException($"Template '{name}' does not exist.");
        }

        private IEnumerable<ITaskTemplate> BuildTemplates()
        {
            yield return new TaskTemplate(
                Templates.Deploy,
                new[]
                {
                    typeof(StopService),
                    typeof(BackupService),
                    typeof(ClearBinaries),
                    typeof(EnsureFolders),
                    typeof(InstallPackage),
                    typeof(DeployHostConfig),
                    typeof(GetConfigSample),
                    typeof(DeployServiceConfig),
                    typeof(StartService),
                });

            yield return new TaskTemplate(
                Templates.Delete,
                new[]
                {
                    typeof(StopService),
                    typeof(UninstallService),
                    typeof(DeleteFolders),
                    typeof(UninstallPackage),
                    typeof(PublishDeploymentDeletedEvent),
                });

            yield return new TaskTemplate(
                Templates.Install,
                new[]
                {
                    typeof(InstallService)
                });

            yield return new TaskTemplate(
                Templates.Uninstall,
                new[]
                {
                    typeof(UninstallService)
                });

            yield return new TaskTemplate(
                Templates.Start,
                new[]
                {
                    typeof(StartService)
                });

            yield return new TaskTemplate(
                Templates.Stop,
                new[]
                {
                    typeof(StopService)
                });

            yield return new TaskTemplate(
                Templates.Restore,
                new[]
                {
                    typeof(StopService),
                    typeof(BackupService),
                    typeof(ClearBinaries),
                    typeof(RestoreFromBackup),
                });

            yield return new TaskTemplate(
                Templates.MoveFolder,
                new[]
                {
                    typeof(StopService),
                    typeof(UninstallService),
                    typeof(MoveFolder),
                    typeof(DeployHostConfig),
                    typeof(InstallService),
                    typeof(StartService),
                });
        }
    }
}