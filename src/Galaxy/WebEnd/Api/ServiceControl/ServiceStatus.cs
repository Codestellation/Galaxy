namespace Codestellation.Galaxy.WebEnd.Api.ServiceControl
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