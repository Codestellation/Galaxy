﻿using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ProvideHostConfig: OperationBase
    {
        const string serviceConfigFileName = "service-config.xml";

        public ProvideHostConfig(string targetPath, Deployment deployment, NugetFeed feed) :
            base(targetPath, deployment, feed)
        {

        }
        public override void Execute()
        {
            string serviceTargetPath = Path.Combine(_targetPath, Deployment.DisplayName);
            string serviceConfigFileNameFull = Path.Combine(serviceTargetPath, serviceConfigFileName);

            var config = new ServiceConfig(Deployment);

            config.Serialize(serviceConfigFileNameFull);
        }
    }
}