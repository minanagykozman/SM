using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Servant
    {
        public int ServantID { get; set; }
        public string ServantName { get; set; } = string.Empty;
        public string Mobile1 { get; set; } = string.Empty;
        public string? Mobile2 { get; set; }
        //public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string UserID { get; set; }
        public ICollection<ServantClass> ServantClasses { get; set; } = new List<ServantClass>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        public ICollection<EventAttendance> EventAttendances { get; set; } = new List<EventAttendance>();
        public ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
        public ICollection<Fund> Funds { get; set; } = new List<Fund>();
    }

}
