using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.EventParams;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;
using NuGet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Codestellation.Galaxy.ServiceManager
{
    public class ServiceControl
    {
        readonly string _targetPath;

        readonly Queue<ServiceOperation> _operations = new Queue<ServiceOperation>();

        readonly Deployment Deployment;

        readonly NugetFeed _feed;

        readonly string _hostPackageFeedUri;
        readonly string _hostPackageName;

        readonly IOperationsFactory _opFactory;

        public ServiceControl(IOperationsFactory opFactory, Deployment deployment, NugetFeed feed)
        {
            _targetPath = ConfigurationManager.AppSettings["appsdestination"];
            _hostPackageFeedUri = ConfigurationManager.AppSettings["hostPackageFeedUri"];
            _hostPackageName = ConfigurationManager.AppSettings["hostPackageName"];

            _feed = feed;
            Deployment = deployment;
            _opFactory = opFactory;
        }

        #region public operations part

        public void AddInstall()
        {
            var deployment = new Deployment
            {
                DisplayName = Deployment.DisplayName
            };

            var hostFeed = new NugetFeed
            {
                Name = _hostPackageName,
                Uri = _hostPackageFeedUri
            };

            _operations.Enqueue(_opFactory.GetInstallPackageOp(_targetPath, deployment, hostFeed));
            _operations.Enqueue(_opFactory.GetInstallPackageOp(_targetPath, Deployment, _feed));
            _operations.Enqueue(_opFactory.GetCopyNugetsToRootOp(_targetPath, Deployment, _feed));
            _operations.Enqueue(_opFactory.GetProvideServiceConfigOp(_targetPath, Deployment, _feed));
            _operations.Enqueue(_opFactory.GetInstallServiceOp(_targetPath, Deployment, _feed));
        }

        public void AddUninstall()
        {
            _operations.Enqueue(_opFactory.GetStopServiceOp(_targetPath, Deployment, _feed));
            _operations.Enqueue(_opFactory.GetUninstallServiceOp(_targetPath, Deployment, _feed));
            _operations.Enqueue(_opFactory.GetUninstallPackageOp(_targetPath, Deployment, _feed));
        }
        public void AddStart()
        {
            _operations.Enqueue(_opFactory.GetStartServiceOp(_targetPath, Deployment, _feed));
        }
        public void AddStop()
        {
            _operations.Enqueue(_opFactory.GetStopServiceOp(_targetPath, Deployment, _feed));
        }       
        #endregion

        public void Operate()
        {
            Task tsk = new Task(
                () =>
                {
                    Queue<ServiceOperation> localQueue = new Queue<ServiceOperation>(_operations);
                    _operations.Clear();

                    OperationResult[] results = new OperationResult[localQueue.Count];
                    int index = 0;
                    while (localQueue.Count > 0)
                    {
                        var operation = localQueue.Dequeue();
                        operation.Execute();
                        results[index++] = operation.Result;
                    }

                    var success = results.FirstOrDefault(item => item == OperationResult.OR_FAIL) == default(OperationResult);

                    CallOnCompleted(Deployment, _feed, "all", success ? OperationResult.OR_OK : OperationResult.OR_FAIL, "");
                });
            tsk.Start();
        }

        void CallOnCompleted(Deployment deployment, NugetFeed feed, string operation, OperationResult result, string details)
        {
            var eventHanler = OnCompleted;
            if (eventHanler != null)
                eventHanler(this, new OperationCompletedEventArgs(deployment, feed, operation, result, details));
        }
        public event EventHandler<OperationCompletedEventArgs> OnCompleted;
    }
}
