using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SM.DAL.DataModel
{
    public class Servant
    {
        public int ServantID { get; set; }
        public string ServantName { get; set; } = string.Empty;
        public string Mobile1 { get; set; } = string.Empty;
        public string? Mobile2 { get; set; }
        public bool IsActive { get; set; }
        public string UserID { get; set; }
        [JsonIgnore]
        public ICollection<ServantClass> ServantClasses { get; set; } = new List<ServantClass>();
        [JsonIgnore]
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        [JsonIgnore]
        public ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
        [JsonIgnore]
        public ICollection<MemberFund> Funds { get; set; } = new List<MemberFund>();
    }

}
