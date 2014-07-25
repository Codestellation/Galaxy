using System;
using System.Text;
using Codestellation.Galaxy.Domain;
using System.Xml;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployUserConfig : OperationBase
    {
        public DeployUserConfig(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {

        }

        public override void Execute(StringBuilder buildLog)
        {
            if (string.IsNullOrEmpty(_deployment.ConfigFileContent))
            {
                throw new InvalidOperationException("Can't deploy missing config.");
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_deployment.ConfigFileContent);

            var configFileName = string.Format("{0}\\{1}.config", ServiceFolder, ServiceHostFileName);
            doc.Save(configFileName);
        }
    }
}
