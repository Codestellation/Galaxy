using Codestellation.Galaxy.ServiceManager.Operations;
using MediatR;

namespace Codestellation.Galaxy.Domain.Notifications
{
    public class OperationProgressNotification : INotification
    {
        public readonly int Index;
        public readonly int Total;
        public readonly OperationResult Result;

        public OperationProgressNotification(int index, int total, OperationResult result)
        {
            Index = index;
            Total = total;
            Result = result;
        }
    }
}