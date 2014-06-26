using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.Tests.ServiceManager.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess(Deployment deployment, NugetFeed deploymentFeed)
        {
            var task = new DeploymentTask(deployment, deploymentFeed, "");
            task.Operations.AddLast(new FakeOpSuccess("", deployment, deploymentFeed));
            return task;
        }
        public static DeploymentTask SequenceTaskFail(Deployment deployment, NugetFeed deploymentFeed)
        {
            var task = new DeploymentTask(deployment, deploymentFeed, "");
            task.Operations.AddLast(new FakeOpFail("", deployment, deploymentFeed));
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle(Deployment deployment, NugetFeed deploymentFeed)
        {
            var task = new DeploymentTask(deployment, deploymentFeed, "");
            task.Operations.AddLast(new FakeOpSuccess("", deployment, deploymentFeed));
            task.Operations.AddLast(new FakeOpFail("", deployment, deploymentFeed));
            task.Operations.AddLast(new FakeOpSuccess("", deployment, deploymentFeed));
            return task;
        }
    }
}
