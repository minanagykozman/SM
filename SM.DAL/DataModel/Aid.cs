using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Aid
    {
        public int AidID { get; set; }
        public string AidName { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<MemberAid> MemberAids { get; set; } = new List<MemberAid>();
    }

}
