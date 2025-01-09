using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MemberAid
    {
        public int AidID { get; set; }
        public Aid Aid { get; set; } = null!;
        public int MemberID { get; set; }
        public Member Member { get; set; } = null!;
        public int ServantID { get; set; }
        public Servant Servant { get; set; } = null!;
        public DateTime TimeStamp { get; set; }
        public string? Notes { get; set; }
    }

}
