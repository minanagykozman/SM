using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ClassMember
    {
        public int ClassID { get; set; }
        public Class Class { get; set; } = null!;
        public int MemberID { get; set; }
        public Member Member { get; set; } = null!;
    }

}
