using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public interface IOperation
    {
        void Execute(TextWriter buildLog);
    }
}