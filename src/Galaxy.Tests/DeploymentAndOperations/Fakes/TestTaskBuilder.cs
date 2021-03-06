using System.IO;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;
using MediatR;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess(IMediator mediator)
        {
            var context = CreateContext("TaskSuccess", mediator);
            var task = new DeploymentTask(context);
            task.Add(new FakeOpSuccess());
            return task;
        }

        public static DeploymentTask SequenceTaskFail(IMediator mediator)
        {
            var context = CreateContext("TaskFail", mediator);
            var task = new DeploymentTask(context);
            task.Add(new FakeOpFail());
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle(IMediator mediator)
        {
            var context = CreateContext("TaskFailInTheMiddle", mediator);
            var task = new DeploymentTask(context);
            task.Add(new FakeOpSuccess());
            task.Add(new FakeOpFail());
            task.Add(new FakeOpSuccess());
            return task;
        }

        public static DeploymentTaskContext CreateContext(string name, IMediator mediator)
        {
            var context = new DeploymentTaskContext()
            {
                TaskName = name,
                Mediator = mediator,
            };

            return context;
        }
    }
}