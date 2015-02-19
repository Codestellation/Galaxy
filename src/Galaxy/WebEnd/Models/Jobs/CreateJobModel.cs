using System;
using System.ComponentModel.DataAnnotations;

namespace Codestellation.Galaxy.WebEnd.Models.Jobs
{
    public class CreateJobModel
    {
        [Display(Name = "Display name", Prompt = "text")]
        public string DisplayName { get; set; }

        [Display(Name = "Schedule", Prompt = "Cron Expression")]
        public string Schedule { get; set; }

        private string _timeZoneId;

        [Display(Name = "Time zone", Prompt = "Select timezone")]
        public string TimeZoneId
        {
            get
            {
                return _timeZoneId ?? TimeZoneInfo.Local.Id;
            }
            set { _timeZoneId = value; }
        }
    }
}