namespace Codestellation.Galaxy.Domain
{
    public static class DeploymentExtensions
    {
        public static string GetDeployFolder(this Deployment self)
        {
            return self.Folders.DeployFolder.ToString();
        }

        public static string GetDeployLogFolder(this Deployment self)
        {
            return self.Folders.DeployLogsFolder.ToString();
        }
    }
}