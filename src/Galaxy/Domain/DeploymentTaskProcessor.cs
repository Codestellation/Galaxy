using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.ServiceManager;
using NLog;

namespace Codestellation.Galaxy.Domain
{
    public class DeploymentTaskProcessor : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlockingCollection<DeploymentTask> _taskQueue;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly Task _processingTask;

        public DeploymentTaskProcessor()
        {
            _taskQueue = new BlockingCollection<DeploymentTask>();
            _cancellationSource = new CancellationTokenSource();
            _processingTask = Task.Run((Action)ProcessInternal);
        }

        public void Process(DeploymentTask task)
        {
            _taskQueue.Add(task);
        }

        private void ProcessInternal()
        {
            DeploymentTask temp = null;
            try
            {
                foreach (var task in _taskQueue.GetConsumingEnumerable(_cancellationSource.Token))
                {
                    temp = task;
                    task.Process();
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