using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int MeetingID { get; set; }
        public Meeting Meeting { get; set; } = null!;
        public bool IsActive { get; set; }

        public ICollection<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();
        public ICollection<ClassOccurrence> ClassOccurrences { get; set; } = new List<ClassOccurrence>();
        public ICollection<ServantClass> ServantClasses { get; set; } = new List<ServantClass>();
        public ICollection<ClassEvent> ClassEvents { get; set; } = new List<ClassEvent>();
    }

}
