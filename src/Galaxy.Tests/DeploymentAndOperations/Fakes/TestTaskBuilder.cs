using System.IO;
using Codestellation.Galaxy.ServiceManager;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess()
        {
            var task = new DeploymentTask("TaskSuccess", new ObjectId(), new MemoryStream());
            task.Add(new FakeOpSuccess());
            return task;
        }

        public static DeploymentTask SequenceTaskFail()
        {
            var task = new DeploymentTask("TaskFail", new ObjectId(), new MemoryStream());
            task.Add(new FakeOpFail());
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle()
        {
            var task = new DeploymentTask("TaskFailInTheMiddle", new ObjectId(), new MemoryStream());
            task.Add(new FakeOpSuccess());
            task.Add(new FakeOpFail());
            task.Add(new FakeOpSuccess());
            return task;
        }
    }
}
