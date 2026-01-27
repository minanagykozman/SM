using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel.APIModels
{
    public class MemberSearchCriteria
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBaptised { get; set; }
        public string? CardStatus { get; set; }
        public List<int>? ClassIDs { get; set; }
        public DateTime? BirthdateStart { get; set; }
        public DateTime? BirthdateEnd { get; set; }
        public bool IsNotInAnyClass { get; set; }
        public bool ClassOperatorIsOr { get; set; }
    }
}
