using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Church
    {
        public int ChurchID { get; set; }
        public string ChurchName { get; set; }
        public string? Address { get; set; }
        public ICollection<ChurchMember> ChurchMembers { get; set; }
        public ICollection<Servant> ChurchServants { get; set; }
        public ICollection<Meeting> ChurchMeetings { get; set; }
    }
}
