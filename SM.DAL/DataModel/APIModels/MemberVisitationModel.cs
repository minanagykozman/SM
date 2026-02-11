using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel.APIModels
{
    public class VisitationModel
    {
        public  MemberVisitationModel MainMember {  get; set; }
        public List<MemberVisitationModel> FamilyMembers { get; set; }
    }
    public class MemberVisitationModel
    {
        public int MemberID { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberCode { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;
        public List<MemberAttendanceSummaryView> Attendance { get; set; }
    }
    
}
