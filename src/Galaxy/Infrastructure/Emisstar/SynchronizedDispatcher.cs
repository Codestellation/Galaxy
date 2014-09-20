using System.Threading;
using System.Threading.Tasks;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Galaxy.Infrastructure.Emisstar
{
    public class SynchronizedDispatcher : RuleBasedDispatcher
    {
        public SynchronizedDispatcher() : base(new Rule(MarkedSynchronized))
        {
            
        }

        protected override void Invoke(ref MessageHandlerTuple tuple)
        {
            MessageHandlerTuple handlerTuple = tuple;
            Task.Factory.StartNew(() => InternalInvoke(handlerTuple), CancellationToken.None, TaskCreationOptions.None, SingleThreadScheduler.Instance);
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
                    .GetCustomAttributes(typeof (SynchronizedAttribute), false)
                    .Length > 0;
        }
    }
}