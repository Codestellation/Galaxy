using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Consul;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    public class ConsulWrapper
    {
        private const string Datadir = "consuldata";
        private Process _consulProcess;
        private AutoResetEvent _consulStarted;
        private Client _client;

        public void Start()
        {
            var directory = new DirectoryInfo(Datadir);
            if (directory.Exists)
            {
                directory.Delete(true);
            }

            var startInfo = new ProcessStartInfo(
                "consulx.exe",
                "agent -server -bootstrap-expect 1 -data-dir " + Datadir);

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            _consulProcess = new Process();
            _consulProcess.StartInfo = startInfo;

            _consulProcess.OutputDataReceived += OnNewLine;
            _consulProcess.ErrorDataReceived += OnNewLine;

            _consulStarted = new AutoResetEvent(false);
            _consulProcess.Start();

            _consulProcess.BeginOutputReadLine();
            _consulProcess.BeginErrorReadLine();

            if (!_consulStarted.WaitOne(TimeSpan.FromSeconds(30)))
            {
                throw new InvalidOperationException("Could not start consul agent");
            }

            _client = new Client();
        }

        private void OnNewLine(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            if (e.Data.Contains("New leader elected"))
            {
                _consulStarted.Set();
            }
        }

        public void Stop()
        {
            _consulProcess.CancelErrorRead();
            _consulProcess.CancelOutputRead();

            _consulProcess.Kill();
            _consulProcess.WaitForExit();
        }

        public void PutKey(string key, object value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var json = new JsonTextWriter(writer);

            new JsonSerializer().Serialize(json, value);
            json.Flush();
            var array = new byte[stream.Position];

            Array.Copy(stream.GetBuffer(), array, array.Length);

            var kvPair = new KVPair(key)
            {
                Value = array
            };
            var putresult = _client.KV.Put(kvPair);

            Console.WriteLine(JsonConvert.SerializeObject(putresult));
        }
    }
}