namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public interface IOperation
    {
        void Execute(DeploymentTaskContext context);
    }
}