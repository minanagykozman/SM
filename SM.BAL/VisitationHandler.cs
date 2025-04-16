using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;
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
        public void AddVisitation(int memberID, string servantUsername, string feedback, string visitationType, int? assignedServantID)
        {
            Servant servant = GetServantByUsername(servantUsername);
            Visitation visitation = new Visitation()
            {
                MemberID = memberID,
                ServantID = servant.ServantID,
                Feedback = feedback,
                VisitaionDate = CurrentTime,
                VisitationType = visitationType,
                Checked = (assignedServantID.HasValue && assignedServantID != 0) ? false : null,
                AssignedServantID = (assignedServantID.HasValue && assignedServantID != 0) ? assignedServantID.Value : null
            };
            _dbcontext.Add(visitation);
            _dbcontext.SaveChanges();
        }
        public void UpdateVisitationFeedback(int visitationID, int assignedServantID,string feedback,bool isChecked)
        {
            Visitation visitation = _dbcontext.Visitations.Where(v=>v.VisitationID == visitationID).FirstOrDefault()!;
            if (visitation == null)
            {
                throw new Exception("Visitation not found");
            }
            visitation.AssignedServantID = assignedServantID;
            visitation.Checked = isChecked;
            visitation.AssignedServantFeedback = feedback;
            visitation.LastModifiedDate = CurrentTime;

            _dbcontext.SaveChanges();
        }
    }
}
