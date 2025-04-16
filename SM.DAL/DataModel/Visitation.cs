using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class Visitation
    {
        public int VisitationID { get; set; }
        public int MemberID { get; set; }
        public string VisitationType { get; set; } = default!;
        public int ServantID {  get; set; }
        public string? Feedback {  get; set; }
        public string? AssignedServantFeedback { get; set; }
        public int? AssignedServantID {  get; set; }
        public bool? Checked {  get; set; }
        public DateTime VisitaionDate {  get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public virtual Member Member { get; set; }
        public virtual Servant Servant { get; set; }
        public virtual Servant? AssignedServant { get; set; }

    }
}
