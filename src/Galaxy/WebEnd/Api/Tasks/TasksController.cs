using System;
using System.Web.Http;

namespace Codestellation.Galaxy.WebEnd.Api.Tasks
{
    /*  To be done
     *  GET /tasks - current queued tasks
     *  GET /tasks/{id} - detailed info about tasks
     *  POST /tasks - add tasks in queue
     *  DELETE /tasks/{id} - remove specified task (conflict it task in progress)
     */
    public class TasksController : ApiController
    {
        public object Get()
        {
            return "Placeholder for all tasks";
        }

        public object Get(Guid id)
        {
            return string.Format("Placeholder for task {0}", id.ToString());
        }

        public object Post()
        {
            return Guid.NewGuid().ToString();
        }

        public object Delete(Guid id)
        {
            return string.Format("Placeholder for delete task {0}", id.ToString());
        }
    }
}