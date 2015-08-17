using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain.Notifications;
using Microsoft.AspNet.SignalR;

namespace Codestellation.Galaxy.WebEnd.SignalR.Alerts
{
    public class AlertHandler : IHandler<OperationProgressNotification>
    {
        private readonly IHubContext _alerthub;

        public AlertHandler(HubFactory factory)
        {
            _alerthub = factory.Create<AlertHub>();
        }

        public void Handle(OperationProgressNotification message)
        {
            var alertModel = AlertModel.From(message);
            _alerthub.Clients.All.onalert(alertModel);
        }
    }
}