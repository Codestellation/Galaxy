using System.IO;
using Codestellation.Emisstar;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;
using Nejdb.Bson;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess(IPublisher publisher)
        {
            var context = CreateContext("TaskSuccess", publisher);
            var task = new DeploymentTask(context);
            task.Add(new FakeOpSuccess());
            return task;
        }

        public static DeploymentTask SequenceTaskFail(IPublisher publisher)
        {
            var context = CreateContext("TaskFail", publisher);
            var task = new DeploymentTask(context);
            task.Add(new FakeOpFail());
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle(IPublisher publisher)
        {
            var context = CreateContext("TaskFailInTheMiddle", publisher);
            var task = new DeploymentTask(context);
            task.Add(new FakeOpSuccess());
            task.Add(new FakeOpFail());
            task.Add(new FakeOpSuccess());
            return task;
        }

        public static DeploymentTaskContext CreateContext(string name, IPublisher publisher)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var context = new DeploymentTaskContext(streamWriter)
                .SetValue(DeploymentTaskContext.TaskName, name)
                .SetValue(DeploymentTaskContext.DeploymentId, new ObjectId())
                .SetValue(DeploymentTaskContext.PublisherKey, publisher)
                .SetValue(DeploymentTaskContext.LogStream, stream);
            return context;
        }
    }
}
