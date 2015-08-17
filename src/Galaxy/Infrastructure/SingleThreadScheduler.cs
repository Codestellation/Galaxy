using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Infrastructure
{
    public class SingleThreadScheduler : TaskScheduler
    {
        public static readonly SingleThreadScheduler Instance = new SingleThreadScheduler();

        private readonly Queue<Task> _tasks;
        private bool _running;

        public SingleThreadScheduler()
        {
            _tasks = new Queue<Task>(10000);
        }

        protected override sealed void QueueTask(Task task)
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
                    else
                    {
                        item = _tasks.Dequeue();
                    }
                }

                TryExecuteTask(item);
            }
        }

        // Do not support inlining
        protected override sealed bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override sealed bool TryDequeue(Task task)
        {
            return false;
        }

        public override sealed int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        protected override sealed IEnumerable<Task> GetScheduledTasks()
        {
            lock (_tasks)
            {
                return _tasks.ToArray();
            }
        }
    }
}