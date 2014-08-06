using Codestellation.Galaxy.Domain;
using System.Xml.Serialization;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    public class ServiceConfig
    {
        [XmlElement]
        public string ServiceName { get; set; }
        [XmlElement]
        public string DisplayName { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
        public string InstanceName { get; set; }

        /// <summary>
        /// ctor for serializer
        /// </summary>
        public ServiceConfig()
        {

        }

        public ServiceConfig(Deployment deployment)
        {
            Description = deployment.Description;
            DisplayName = deployment.DisplayName;
            ServiceName = deployment.ServiceName;
            InstanceName = deployment.InstanceName;
        }

        public void Serialize(string fileName)
        {
            using (var writer = new System.IO.StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }            
        }
    }
}
