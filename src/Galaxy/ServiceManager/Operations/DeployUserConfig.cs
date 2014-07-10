using System;
using Codestellation.Galaxy.Domain;
using System.IO;
using System.Xml;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployUserConfig : OperationBase
    {
        public DeployUserConfig(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(_deployment.ConfigFileContent))
            {
                throw new InvalidOperationException("Can't deploy missing config.");
            }

            string serviceTargetPath = Path.Combine(_targetPath, _deployment.DisplayName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_deployment.ConfigFileContent);

            var configFileName = string.Format("{0}\\{1}.config", serviceTargetPath, ServiceHostFileName);
            doc.Save(configFileName);
        }
    }
}
