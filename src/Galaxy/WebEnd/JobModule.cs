using Codestellation.Galaxy.WebEnd.Models.Jobs;

namespace Codestellation.Galaxy.WebEnd
{
    public class JobModule : CrudModule
    {
        public const string Path = "job";

        public JobModule() : base(Path)
        {
        }

        protected override CrudOperations SupportedOperations
        {
            get { return CrudOperations.GetList | CrudOperations.GetCreate;}
        }

        protected override object GetList(dynamic parameters)
        {
            return new JobListModel();
        }

        protected override object GetCreate(dynamic parameters)
        {
            return new CreateJobModel();
        }
    }
}