using SM.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DAL.DataModel
{
    public class ClassMemberExtended : Member
    {
        public ClassMemberExtended() { }
        public ClassMemberExtended(Member member)
        {
            this.MemberID = member.MemberID;
            this.UNFileNumber = member.UNFileNumber;
            this.UNPersonalNumber = member.UNPersonalNumber;
            this.UNFirstName = member.UNFirstName;
            this.UNLastName = member.UNLastName;
            this.Baptised = member.Baptised;
            this.CardStatus = member.CardStatus;
            this.Code = member.Code;
            this.CreatedAt = member.CreatedAt;
            this.Gender = member.Gender;
            this.Birthdate = member.Birthdate;
            this.ImageReference = member.ImageReference;
            this.ImageURL = member.ImageURL;
            this.IsActive = member.IsActive;
            this.Mobile = member.Mobile;
            this.ModifiedAt = member.ModifiedAt;
            this.ModifiedBy = member.ModifiedBy;
            this.Nickname = member.Nickname;
            this.Work = member.Work;
            this.CreatedBy = member.CreatedBy;
            this.School = member.School;
            this.Sequence = member.Sequence;
            this.LastVisitationType = member.LastVisitationType;
            this.LastVisitationDate = member.LastVisitationDate;
        }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastPresentDate { get; set; } = null;
        public string Attendance { get; set; } = string.Empty;
        public string? Servant { get; set; } = string.Empty;
        public int AttendanceCounter{ get; set; }
    }
}
