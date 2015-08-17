using System;
using System.IO;
using Consul;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Host.ConfigManagement
{
    public class ConfigLoader
    {
        private readonly Client _client;

        public ConfigLoader(Client client)
        {
            _client = client;
        }

        public void Load(ConfigElement element)
        {
            var kvs = _client.KV;

            var x = kvs.Get(element.Path);

            Console.WriteLine(JsonConvert.SerializeObject(x, Formatting.Indented));
            if (x.Response == null)
            {
                element.ValueMissed();
            }
            else
            {
                byte[] rawValue = x.Response.Value;

                try
                {
                    using (var stream = new MemoryStream(rawValue))
                    using (var streamReader = new StreamReader(stream))
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        var type = element.Type;

                        var value = new JsonSerializer().Deserialize(jsonReader, type);

                        element.ValueFound(rawValue, value);
                    }
                }
                catch (Exception ex)
                {
                    element.ValueInvalid(rawValue, ex.Message);
                }
            }
        }
    }
}