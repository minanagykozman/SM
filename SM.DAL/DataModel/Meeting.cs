using System;
using System.Collections.Generic;
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
        public DateTime? MeetingStartDate { get; set; }
        public DateTime? MeetingEndDate { get; set; }
        public string MeetingDay { get; set; } = string.Empty;
        public string MeetingStartTime { get; set; } = string.Empty;
        public string MeetingEndTime { get; set; } = string.Empty;
        public string MeetingFrequency { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public ICollection<Class> Classes { get; set; } = new List<Class>();
    }

}
