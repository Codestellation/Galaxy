using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess()
        {
            var deployment = GetDeployment();
            var task = new DeploymentTask("TaskSuccess", deployment, string.Empty);
            task.Add(new FakeOpSuccess(string.Empty, deployment));
            return task;
        }

        public static DeploymentTask SequenceTaskFail()
        {
            var deployment = GetDeployment();
            var task = new DeploymentTask("TaskFail", deployment, "");
            task.Add(new FakeOpFail(string.Empty, deployment));
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle()
        {
            var deployment = GetDeployment();

            var task = new DeploymentTask("TaskFailInTheMiddle", deployment, "");
            task.Add(new FakeOpSuccess(string.Empty, deployment));
            task.Add(new FakeOpFail(string.Empty, deployment));
            task.Add(new FakeOpSuccess(string.Empty, deployment));
            return task;
        }

        private static Deployment GetDeployment()
        {
            return new Deployment { DisplayName = "FooService" };
        }
    }
}
