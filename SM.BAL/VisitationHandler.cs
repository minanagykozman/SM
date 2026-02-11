using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;
using SM.DAL.DataModel.APIModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace SM.BAL
{
    public class VisitationHandler : HandlerBase
    {
        public List<Visitation> AddVisitation(List<int> memberIDs, string servantUsername, string feedback, string visitationType, int? assignedServantID, int? classID)
        {
            Servant servant = GetServantByUsername(servantUsername);
            List<Visitation> visitations = new List<Visitation>();
            foreach (int memberID in memberIDs)
            {
                Visitation visitation = new Visitation()
                {
                    ServantID = servant.ServantID,
                    Feedback = feedback,
                    CreatedAt = CurrentTime,
                    VisitationType = visitationType,
                    AssignedServantID = (assignedServantID.HasValue && assignedServantID != 0) ? assignedServantID.Value : null,
                    Status = "Assigned",
                    ClassID = classID,
                    MemberID = memberID
                };
                _dbcontext.Add(visitation);
                _dbcontext.SaveChanges();
                visitations.Add(visitation);
            }

            return visitations;
        }
        public Visitation UpdateVisitationFeedback(int visitationID, List<int> memberIDs, string feedback, string status, string visitationType, DateTime? visitationDate)
        {
            Visitation visitation = _dbcontext.Visitations.Where(v => v.VisitationID == visitationID).FirstOrDefault()!;
            if (visitation == null)
            {
                throw new Exception("Visitation not found");
            }
            visitation.AssignedServantFeedback = feedback;
            visitation.LastModifiedDate = CurrentTime;
            visitation.Status = status;
            visitation.VisitationType = visitationType;
            visitation.VisitaionDate = visitationDate.Value;

            if (memberIDs != null && (status == "Done" || status == "Follow up needed"))
            {
                foreach (int memberID in memberIDs)
                {
                    if (memberID == visitation.MemberID)
                        continue;
                    Visitation newVisitaion = new Visitation()
                    {
                        VisitaionDate = visitationDate.Value,
                        AssignedServantID = visitation.AssignedServantID,
                        VisitationType = visitation.VisitationType,
                        Status = status,
                        CreatedAt = CurrentTime,
                        ServantID = visitation.AssignedServantID.Value,
                        MemberID = memberID
                    };
                    _dbcontext.Visitations.Add(newVisitaion);
                }
            }
            _dbcontext.SaveChanges();
            return visitation;
        }

        public List<Visitation> GetVisitations(string? servantUsername)
        {
            var query = _dbcontext.Visitations.AsQueryable();
            if (!string.IsNullOrEmpty(servantUsername))
            {
                Servant servant = GetServantByUsername(servantUsername);
                query = query.Where(v => v.ServantID == servant.ServantID || v.AssignedServantID == servant.ServantID);
            }
            var visitations = query.Include(v => v.Member).Include(v => v.Servant).Include(v => v.AssignedServant).Include(v => v.Class).ToList();

            return visitations;
        }
        public VisitationModel GetMemberFamilyAttendance(int memberID)
        {
            var member = _dbcontext.Members.Where(m => m.MemberID == memberID).FirstOrDefault();
            var attendanceData = _dbcontext.MemberAttendanceSummaryView.Where(m => m.UNFileNumber == member.UNFileNumber).ToList();
            var familyMembers = _dbcontext.Members.Where(m => m.UNFileNumber == member.UNFileNumber && m.MemberID != memberID).ToList();
            VisitationModel model = new VisitationModel();
            model.FamilyMembers = new List<MemberVisitationModel>();
            model.MainMember = new MemberVisitationModel();

            model.MainMember.MemberID = memberID;
            model.MainMember.MemberCode = member.Code;
            model.MainMember.MemberName = member.FullName;
            model.MainMember.Mobile = member.Mobile;
            model.MainMember.Attendance = attendanceData.Where(m => m.MemberID == memberID).ToList();

            foreach (var familyMember in familyMembers)
            {
                var memberVisitationModel = new MemberVisitationModel();
                memberVisitationModel.MemberID = familyMember.MemberID;
                memberVisitationModel.MemberCode = familyMember.Code;
                memberVisitationModel.MemberName = familyMember.FullName;
                memberVisitationModel.Mobile = familyMember.Mobile;
                memberVisitationModel.Attendance = attendanceData.Where(m => m.MemberID == familyMember.MemberID).ToList();
                model.FamilyMembers.Add(memberVisitationModel);
            }

            return model;
        }
    }
}
