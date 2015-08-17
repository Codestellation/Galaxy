using System.Threading;
using System.Threading.Tasks;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Galaxy.Infrastructure.Emisstar
{
    public class MultiThreadDispatcher : RuleBasedDispatcher
    {
        public MultiThreadDispatcher()
            : base(new Rule(x => true))
        {
        }

        protected override void Invoke(ref MessageHandlerTuple tuple)
        {
            MessageHandlerTuple handlerTuple = tuple;
            if (MarkedSynchronized(tuple))
            {
                Task.Factory.StartNew(() => InternalInvoke(handlerTuple), CancellationToken.None, TaskCreationOptions.None, SingleThreadScheduler.Instance);
            }
            else
            {
                Task.Run(() => InternalInvoke(handlerTuple));
            }
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