using System.Text;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public interface IOperation
    {
        void Execute(StringBuilder buildLog);
    }
}