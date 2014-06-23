using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.EventParams;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;
using NuGet;
using System;
using System.Collections.Generic;
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

        readonly ServiceApp _serviceApp;

        readonly NugetFeed _feed;

        public ServiceControl(string targetPath, ServiceApp serviceApp, NugetFeed feed)
        {
            this._targetPath = targetPath;
            this._feed = feed;
            this._serviceApp = serviceApp;
        }

        #region public operations part

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostServiceApp"> service host app</param>
        /// <param name="hostFeed"> service host feed </param>
        public void AddInstall(ServiceApp hostServiceApp, NugetFeed hostFeed)
        {
            _operations.Enqueue(new InstallPackage(_targetPath, hostServiceApp, hostFeed));
            _operations.Enqueue(new InstallPackage(_targetPath, _serviceApp, _feed));
            _operations.Enqueue(new CopyNugetsToRoot(_targetPath, _serviceApp, _feed));
            _operations.Enqueue(new ProvideServiceConfig(_targetPath, _serviceApp, _feed));
            _operations.Enqueue(new InstallService(_targetPath, _serviceApp, _feed));
        }
        public void AddUninstall()
        {
            _operations.Enqueue(new StopService(_targetPath, _serviceApp, _feed));
            _operations.Enqueue(new UninstallService(_targetPath, _serviceApp, _feed));
            _operations.Enqueue(new UninstallPackage(_targetPath, _serviceApp, _feed));
        }
        public void AddStart()
        {
            _operations.Enqueue(new StartService(_targetPath, _serviceApp, _feed));
        }
        public void AddStop()
        {
            _operations.Enqueue(new StopService(_targetPath, _serviceApp, _feed));
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

                    CallOnCompleted(_serviceApp, _feed, "all", success ? OperationResult.OR_OK : OperationResult.OR_FAIL, "");
                });
            tsk.Start();
        }

        void CallOnCompleted(ServiceApp serviceApp, NugetFeed feed, string operation, OperationResult result, string details)
        {
            var eventHanler = OnCompleted;
            if (eventHanler != null)
                eventHanler(this, new OperationCompletedEventArgs(serviceApp, feed, operation, result, details));
        }
        public event EventHandler<OperationCompletedEventArgs> OnCompleted;
    }
}
