using System.Threading;
using System.Threading.Tasks;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Galaxy.Infrastructure.Emisstar
{
    public class MultiThreadDispatcher : RuleBasedDispatcher
    {
        private readonly SingleThreadScheduler _preserveOrderScheduler;

        public MultiThreadDispatcher()
            : base(new Rule(x => true))
        {
            _preserveOrderScheduler = new SingleThreadScheduler();
        }

        protected override void Invoke(ref MessageHandlerTuple tuple)
        {
            MessageHandlerTuple handlerTuple = tuple;
            var scheduler = _preserveOrderScheduler;

            if (MarkedSynchronized(tuple))
            {
                scheduler = SingleThreadScheduler.Instance;
            }
            Task.Factory.StartNew(() => InternalInvoke(handlerTuple), CancellationToken.None, TaskCreationOptions.None, scheduler);
        }

        private void InternalInvoke(MessageHandlerTuple handlerTuple)
        {
            base.Invoke(ref handlerTuple);
        }

        private static bool MarkedSynchronized(MessageHandlerTuple tuple)
        {
            //cache it later
            return
                tuple
                    .Message
                    .GetType()
                    .GetCustomAttributes(typeof(SynchronizedAttribute), false)
                    .Length > 0;
        }
    }
}