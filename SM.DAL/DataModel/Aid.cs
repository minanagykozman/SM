using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Aid
    {
        public int AidID { get; set; }
        [MaxLength(50)]
        public string AidName { get; set; } = string.Empty;
        public string Components { get; set; } = string.Empty;
        public decimal CostPerPerson { get; set; }
        public decimal TotalCost { get; set; }
        public int PlannedMembersCount { get; set; }
        public int ActualMembersCount { get; set; }
        public DateTime AidDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
    }

}
