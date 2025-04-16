using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ClassMember
    {
        public int ClassID { get; set; }
        public int MemberID { get; set; }
        public int? ServantID { get; set; }
        public virtual Member Member { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
        public virtual Servant Servant { get; set; } = null!;
    }

}
