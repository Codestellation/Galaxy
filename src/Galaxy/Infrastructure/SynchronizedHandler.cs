using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel;
using MediatR;

namespace Codestellation.Galaxy.Infrastructure
{
    public class SynchronizedHandler<TRequest> :
        IAsyncRequestHandler<TRequest>
        where TRequest : IRequest
    {
        private readonly IKernel _kernel;

        public SynchronizedHandler(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        Task IAsyncRequestHandler<TRequest>.Handle(TRequest message)
        {
            var handler = _kernel.Resolve<IRequestHandler<TRequest>>();
            return Task.Factory.StartNew(
                () => handler.Handle(message),
                CancellationToken.None,
                TaskCreationOptions.RunContinuationsAsynchronously,
                SingleThreadScheduler.MainScheduler);
        }
    }

    public class SynchronizedHandler<TRequest, TResponse> :
        IAsyncRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IKernel _kernel;

        public SynchronizedHandler(IKernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        public Task<TResponse> Handle(TRequest message)
        {
            var handler = _kernel.Resolve<IRequestHandler<TRequest, TResponse>>();

            return Task.Factory.StartNew(
                () => handler.Handle(message),
                CancellationToken.None,
                TaskCreationOptions.RunContinuationsAsynchronously,
                SingleThreadScheduler.MainScheduler);
        }
    }
}