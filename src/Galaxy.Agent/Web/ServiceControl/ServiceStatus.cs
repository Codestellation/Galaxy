namespace Galaxy.Agent
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