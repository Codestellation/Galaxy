using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.Infrastructure;
using Nancy;

namespace Codestellation.Galaxy.WebEnd
{
    public abstract class ModuleBase : NancyModule
    {
        public ModuleBase()
        {

        }

        public ModuleBase(string modulePath) : base(modulePath)
        {
            
        }

        protected Task<object> ProcessRequest(Func<object> processor, CancellationToken cancellationToken)
        {
            Context.Culture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            return Task.Factory.StartNew(processor, cancellationToken, TaskCreationOptions.None, Scheduler);
        }

        public TaskScheduler Scheduler
        {
            get { return SingleThreadScheduler.Instance; }
        }
    }
}