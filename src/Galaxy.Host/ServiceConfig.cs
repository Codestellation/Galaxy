namespace Codestellation.Galaxy.Host
{
    public class ServiceConfig
    {
        public string AssemblyQualifiedType { get; set; }
        
        public string ServiceName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string StartMethod
        {
            get { return "Start"; }
        }

        public string StopMethod
        {
            get { return "Stop"; }
        }
    }
}