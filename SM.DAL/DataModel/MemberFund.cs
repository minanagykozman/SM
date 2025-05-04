using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class MemberFund
    {
        public int FundID { get; set; }
        public int MemberID { get; set; }
        public string FundCategory { get; set; }
        public int ServantID { get; set; }
        public int ApproverID { get; set; }
        public decimal RequestedAmount { get; set; }
        public string RequestDescription { get; set; }
        public decimal ApprovedAmount { get; set; }
        public string ApproverNotes{ get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public virtual Member Member { get; set; }
        public virtual Servant Servant { get; set; }
        public virtual Servant? Approver { get; set; }
    }
}
