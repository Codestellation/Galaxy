using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models.Jobs
{
    public class JobListItemModel
    {
        public ObjectId Id { get; set; }
        
        public object Name { get; set; }
    }
}