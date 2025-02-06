using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ClassAttendance
    {
        public int ClassOccurrenceID { get; set; }
        public int MemberID { get; set; }
        public int ServantID { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual ClassOccurrence ClassOccurrence { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
        public virtual Servant Servant { get; set; } = null!;
    }

}
