using System;
using Microsoft.Owin.Hosting;

namespace Galaxy.Agent
{
    public class AgentService
    {
        private IDisposable _owinHost;

        public void Start()
        {
            var baseAddress = string.Format("http://*:{0}", 8089);
            _owinHost = WebApp.Start<OwinStartup>(baseAddress);
        }

        public void Stop()
        {
            if (_owinHost == null)
            {
                return;
            }
            _owinHost.Dispose();
        }
    }
}