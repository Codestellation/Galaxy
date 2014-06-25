using Codestellation.Galaxy.Domain;
using System.Xml.Serialization;

namespace Codestellation.Galaxy.ServiceManager.Helpers
{
    [XmlRoot("ServiceConfig")]
    public class ServiceAppSerializeable
    {
        [XmlElement]
        public string AssemblyQualifiedType { get; set; }
        [XmlElement]
        public string ServiceName { get; set; }
        [XmlElement]
        public string DisplayName { get; set; }
        [XmlElement]
        public string Description { get; set; }

        /// <summary>
        /// ctor for serializer
        /// </summary>
        public ServiceAppSerializeable()
        {

        }

        public ServiceAppSerializeable(ServiceApp serviceApp)
        {
            this.AssemblyQualifiedType = serviceApp.AssemblyQualifiedType;
            this.Description = serviceApp.Description;
            this.DisplayName = serviceApp.DisplayName;
            this.ServiceName = serviceApp.ServiceName;
        }

        public void Serialize(string fileName)
        {
            using (var writer = new System.IO.StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }            
        }
    }
}
