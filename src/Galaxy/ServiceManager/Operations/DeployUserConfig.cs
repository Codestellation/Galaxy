using System.IO;
using System.Xml;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployUserConfig : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _serviceHostFileName;
        private readonly string _content;

        public DeployUserConfig(string serviceFolder, string serviceHostFileName, string content)
        {
            _serviceFolder = serviceFolder;
            _serviceHostFileName = serviceHostFileName;
            _content = content;
        }

        public void Execute(TextWriter buildLog)
        {
            if (string.IsNullOrEmpty(_content))
            {
                buildLog.WriteLine("No config file found. Skipped.");
               return;
            }

            var doc = new XmlDocument();
            doc.LoadXml(_content);

            var configFileName = string.Format("{0}\\{1}.config", _serviceFolder, _serviceHostFileName);
            doc.Save(configFileName);
        }
    }
}
