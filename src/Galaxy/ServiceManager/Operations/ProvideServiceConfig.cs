using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class ProvideServiceConfig: ServiceOperation
    {
        const string serviceConfigFileName = "service-config.xml";

        public ProvideServiceConfig(string targetPath, ServiceApp serviceApp, NugetFeed feed) :
            base(targetPath, serviceApp, feed)
        {

        }
        public override void Execute()
        {
            try
            {
	            string serviceTargetPath = Path.Combine(_targetPath, _serviceApp.DisplayName);
	            string serviceConfigFileNameFull = Path.Combine(serviceTargetPath, serviceConfigFileName);
	
	            var config = new ServiceAppSerializeable(_serviceApp);
		            
	            config.Serialize(serviceConfigFileNameFull);
	            StoreResult(OperationResult.OR_OK, "");
            }
            catch (System.Exception ex)
            {
                StoreResult(OperationResult.OR_FAIL, ex.Message);
            }
        }
    }
}
