using System.IO;
using Codestellation.Emisstar;
using Codestellation.Galaxy.ServiceManager;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess(IPublisher publisher)
        {
            var task = new DeploymentTask("TaskSuccess", new ObjectId(), new MemoryStream(), publisher);
            task.Add(new FakeOpSuccess());
            return task;
        }

        public static DeploymentTask SequenceTaskFail(IPublisher publisher)
        {
            var task = new DeploymentTask("TaskFail", new ObjectId(), new MemoryStream(), publisher);
            task.Add(new FakeOpFail());
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle(IPublisher publisher)
        {
            var task = new DeploymentTask("TaskFailInTheMiddle", new ObjectId(), new MemoryStream(), publisher);
            task.Add(new FakeOpSuccess());
            task.Add(new FakeOpFail());
            task.Add(new FakeOpSuccess());
            return task;
        }
    }
}
