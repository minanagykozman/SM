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
        public string MeetingName { get; set; } = string.Empty;
        public DateTime AgeStartDate { get; set; }
        public DateTime AgeEndDate { get; set; }
        public char Gender { get; set; }= 'A';
        public DateTime? MeetingStartDate { get; set; }
        public DateTime? MeetingEndDate { get; set; }
        public string MeetingDay { get; set; } = string.Empty;
        public string MeetingStartTime { get; set; } = string.Empty;
        public string MeetingEndTime { get; set; } = string.Empty;
        public string MeetingFrequency { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int ChurchID{ get; set; }
        public string? Notes { get; set; }
        public int Year { get; set; } = 2025;
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public Church Church { get; set; }
    }

}
