namespace Codestellation.Galaxy.Agent.Web.ServiceControl
{
    public class ServiceStatus
    {
        public ServiceStatus()
        {
            SystemInformation = new SystemInfo();
        }

        public SystemInfo SystemInformation { get; set; }
    }
}