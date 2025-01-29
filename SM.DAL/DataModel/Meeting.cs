using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Meeting
    {
        public int MeetingID { get; set; }
        [Required]
        public string MeetingName { get; set; } = string.Empty;

        [Display(Name = "Age Group From")]
        [DataType(DataType.Date)]
        public DateTime AgeStartDate { get; set; }

        [Display(Name = "Age Group To")]
        [DataType(DataType.Date)]
        public DateTime AgeEndDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? MeetingStartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? MeetingEndDate { get; set; }
        public string MeetingDay { get; set; } = string.Empty;
        
        public string MeetingStartTime { get; set; } = string.Empty;
        public string MeetingEndTime { get; set; } = string.Empty;
        public string MeetingFrequency { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }

}
