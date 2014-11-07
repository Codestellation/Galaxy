using System;
using System.Diagnostics;
using Codestellation.Quarks.Reflection;

namespace Codestellation.Galaxy.Agent.Web.ServiceControl
{
    public class SystemInfo
    {
        private static readonly Process _process;
        private static readonly DateTime _startedAt;
        private static readonly string _serviceVertion;

        static SystemInfo()
        {
            _process = Process.GetCurrentProcess();
            _startedAt = _process.StartTime.ToUniversalTime();
            _serviceVertion = AssemblyVersion.InformationalVersion;
        }

        public SystemInfo()
        {
            StartedAt = _startedAt;
            Host = Environment.MachineName;
            ServiceVersion = _serviceVertion;
            SystemVersion  = Environment.OSVersion;
            RunTimeVersion = Environment.Version;
            ProcessorCount = Environment.ProcessorCount;
        }

        public DateTime StartedAt { get; set; }
        public string Host { get; set; }
        public Version RunTimeVersion { get; set; }
        public OperatingSystem SystemVersion { get; set; }
        public string ServiceVersion { get; set; }
        public int ProcessorCount { get; set; }
    }
}