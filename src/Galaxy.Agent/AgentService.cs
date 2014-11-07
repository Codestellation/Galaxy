using System;
using System.IO;
using Codestellation.Quarks.IO;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;

namespace Galaxy.Agent
{
    public class AgentService
    {
        private IDisposable _owinHost;

        public void Start()
        {
            var configPath = Folder.Combine("data", "config.json");
            var configContent = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<AgentConfig>(configContent);
            var baseAddress = string.Format("http://*:{0}", config.WebPort);

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