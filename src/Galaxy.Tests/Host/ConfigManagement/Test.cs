using System.Reflection;

namespace Codestellation.Galaxy.Tests.Host.ConfigManagement
{
    public class Test
    {
        public static readonly string ConsulName = "company.environment.product.service";

        public static readonly PropertyInfo PortProperty = typeof(SampleConfig).GetProperty("Port");

        public static readonly string PortKey = PortProperty.Name.ToLowerInvariant();

        public static readonly PropertyInfo HostProperty = typeof(SampleConfig).GetProperty("Host");

        public static readonly string HostKey = HostProperty.Name.ToLowerInvariant();
    }
}