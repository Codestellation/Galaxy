using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Codestellation.Galaxy.WebEnd.SignalR.Alerts
{
    [HubName("alerthub")]
    public class AlertHub : Hub
    {
    }
}