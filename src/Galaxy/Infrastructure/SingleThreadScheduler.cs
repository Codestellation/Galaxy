using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Infrastructure
{
    public class SingleThreadScheduler : TaskScheduler
    {
        public static readonly SingleThreadScheduler MainScheduler = new SingleThreadScheduler();

        private readonly Queue<Task> _tasks;
        private bool _running;

        public SingleThreadScheduler()
        {
            _tasks = new Queue<Task>(100);
        }

        protected sealed override void QueueTask(Task task)
        {
            lock (_tasks)
            {
                _tasks.Enqueue(task);
                if (_running)
                {
                    return;
                }

                _running = true;

                ThreadPool.UnsafeQueueUserWorkItem(StartExecution, null);
            }
        }

        private void StartExecution(object ignore)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            while (true)
            {
                Task item;
                lock (_tasks)
                {
                    if (_tasks.Count == 0)
                    {
                        _running = false;
                        break;
                    }
                    item = _tasks.Dequeue();
                }

                TryExecuteTask(item);
            }
        }

        // Do not support inlining
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

        protected sealed override bool TryDequeue(Task task) => false;

        public sealed override int MaximumConcurrencyLevel => 1;

        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            lock (_tasks)
            {
                return _tasks.ToArray();
            }
        }
    }
}