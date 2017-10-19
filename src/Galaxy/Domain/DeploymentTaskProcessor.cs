using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Events;
using NLog;

namespace Codestellation.Galaxy.Domain
{
    public class DeploymentTaskProcessor : IDisposable
    {
        private readonly TaskBuilder _builder;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlockingCollection<DeploymentTaskRequest> _taskQueue;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly Task _processingTask;

        public DeploymentTaskProcessor(TaskBuilder builder)
        {
            _builder = builder;
            _taskQueue = new BlockingCollection<DeploymentTaskRequest>();
            _cancellationSource = new CancellationTokenSource();
            _processingTask = Task.Run((Action)ProcessInternal);
        }

        public void Enqueue(DeploymentTaskRequest request)
        {
            _taskQueue.Add(request);
        }

        private void ProcessInternal()
        {
            DeploymentTask temp = null;
            try
            {
                foreach (DeploymentTaskRequest request in _taskQueue.GetConsumingEnumerable(_cancellationSource.Token))
                {
                    temp = _builder.Build(request);
                    temp.Process();
                }
            }
            catch (OperationCanceledException)
            {
                //Operation cancelled by cancellation source
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error processing task '{0}'", temp.Name);
            }
        }

        public void Dispose()
        {
            if (_cancellationSource.IsCancellationRequested)
            {
                return;
            }

            _cancellationSource.Cancel();
            _processingTask.Wait();
        }
    }
}