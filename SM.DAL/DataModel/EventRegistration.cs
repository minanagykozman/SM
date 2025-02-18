using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class EventRegistration
    {
        public int EventID { get; set; }
        public Event Event { get; set; } = null!;
        public int MemberID { get; set; }
        public int ServantID { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? Notes { get; set; }
        public bool IsException { get; set; }
        public bool? Attended { get; set; }
        public int? AttendanceServantID { get; set; }
        public DateTime? AttendanceTimeStamp { get; set; }
        [MaxLength(25)]
        public string? Team { get; set; } = null;
        public virtual Member Member { get; set; } = null!;
        public virtual Servant Servant { get; set; } = null!;
    }

}
