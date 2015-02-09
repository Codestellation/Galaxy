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
            get { return CrudOperations.GetList;}
        }

        protected override object GetList(dynamic parameters)
        {
            return new JobListModel();
        }
    }
}