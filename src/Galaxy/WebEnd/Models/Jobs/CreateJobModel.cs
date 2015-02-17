using System.ComponentModel.DataAnnotations;

namespace Codestellation.Galaxy.WebEnd.Models.Jobs
{
    public class CreateJobModel
    {
        [Display(Name = "Schedule", Prompt = "Cron Expression")]
        public string Schedule { get; set; }
    }
}