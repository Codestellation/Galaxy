using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Codestellation.Galaxy.WebEnd.SignalR
{
    public class HubFactory
    {
        public IHubContext Create<THub>()
            where THub : IHub
        {
            return GlobalHost.ConnectionManager.GetHubContext<THub>();
        }
    }
}