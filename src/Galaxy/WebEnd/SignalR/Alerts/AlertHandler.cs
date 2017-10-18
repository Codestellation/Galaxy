using System.Threading.Tasks;
using Codestellation.Galaxy.Domain.Notifications;
using MediatR;
using Microsoft.AspNet.SignalR;

namespace Codestellation.Galaxy.WebEnd.SignalR.Alerts
{
    public class AlertHandler : IAsyncNotificationHandler<OperationProgressNotification>
    {
        private readonly IHubContext _alerthub;

        public AlertHandler(HubFactory factory)
        {
            _alerthub = factory.Create<AlertHub>();
        }

        public Task Handle(OperationProgressNotification notification)
        {
            var alertModel = AlertModel.From(notification);
            _alerthub.Clients.All.onalert(alertModel);
            return Task.CompletedTask;
        }
    }
}