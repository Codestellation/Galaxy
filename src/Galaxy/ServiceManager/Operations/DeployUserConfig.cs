﻿using System;
using System.IO;
using System.Text;
using Codestellation.Galaxy.Domain;
using System.Xml;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployUserConfig : OperationBase
    {
        public DeployUserConfig(string basePath, Deployment deployment) :
            base(basePath, deployment)
        {

        }

        public override void Execute(TextWriter buildLog)
        {
            if (string.IsNullOrEmpty(Deployment.ConfigFileContent))
            {
                throw new InvalidOperationException("Can't deploy missing config.");
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Deployment.ConfigFileContent);

            var configFileName = string.Format("{0}\\{1}.config", ServiceFolder, ServiceHostFileName);
            doc.Save(configFileName);
        }
    }
}
