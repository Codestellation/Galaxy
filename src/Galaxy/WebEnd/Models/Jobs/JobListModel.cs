namespace Codestellation.Galaxy.WebEnd.Models.Jobs
{
    public class JobListModel
    {
        public JobListModel()
        {
            Jobs = new JobListItemModel[0];
        }

        public int Count { get; set; }

        public JobListItemModel[] Jobs { get; set; }
    }
}