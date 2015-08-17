using Codestellation.Galaxy.Domain.Notifications;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.WebEnd.SignalR.Alerts
{
    public class AlertModel
    {
        public string Severity;
        public string Header;
        public string Message;

        public static AlertModel From(OperationProgressNotification message)
        {
            var model = new AlertModel();
            OperationResult result = message.Result;
            switch (result.ResultCode)
            {
                case ResultCode.Succeed:
                    model.Severity = "success";
                    break;
                case ResultCode.Failed:
                    model.Severity = "danger";
                    break;
                case ResultCode.NotRan:
                    model.Severity = "warning";
                    break;
            }

            model.Message = result.Details;

            var header = string.Format("{0} of {1}: {2}", message.Index, message.Total, result.OperationName);
            model.Header = header;
            return model;
        }
    }
}