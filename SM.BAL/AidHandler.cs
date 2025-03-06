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
    public class AidHandler : HandlerBase
    {
        public int CreateAid(Aid aid)
        {
            Aid aidNew = new Aid();
            aidNew.AidName = aid.AidName;
            aidNew.AidDate = aid.AidDate;
            aidNew.CostPerPerson = aid.CostPerPerson;
            aidNew.ActualMembersCount = aid.ActualMembersCount;
            aidNew.PlannedMembersCount = aid.PlannedMembersCount;
            aidNew.TotalCost = aid.TotalCost;
            aidNew.IsActive = true;
            _dbcontext.Aids.Add(aidNew);
            _dbcontext.SaveChanges();
            return aidNew.AidID;
        }

        public AidStatus CheckMemberStatus(int aidID, string memberCode,out Member member)
        {
            Member? lmember = GetMember(memberCode);
            member = lmember;
            if (lmember == null)
            {
                return AidStatus.MemberNotFound;
            }
            Aid? aid = _dbcontext.Aids.Where(a => a.AidID == aidID).FirstOrDefault(); ;
            if (aid == null)
            {
                return AidStatus.AidNotFound;
            }
            if (_dbcontext.MemberAids.Any(m => m.AidID == aidID && m.MemberID == lmember.MemberID))
            {
                return AidStatus.AlreadyTook;
            }
            List<int> aidClasses = _dbcontext.AidClasses.Where(e => e.AidID == aid.AidID).Select(e => e.ClassID).ToList<int>();
            List<int> memberClasses = _dbcontext.ClassMembers.Where(m => m.MemberID == lmember.MemberID).Select(e => e.ClassID).ToList<int>();
            if (aidClasses == null || memberClasses == null)
                return AidStatus.NotEligible;
            if (!aidClasses.Intersect(memberClasses).Any())
                return AidStatus.NotEligible;

            return AidStatus.Eligible;
        }
        public AidStatus TakeAid(int aidID, string memberCode, string notes, string username)
        {
            Servant servant = GetServantByUsername(username);
            Member? member = GetMember(memberCode);
            if (member == null)
            {
                return AidStatus.MemberNotFound;
            }
            Aid? aid = _dbcontext.Aids.Where(a => a.AidID == aidID).FirstOrDefault(); ;
            if (aid == null)
            {
                return AidStatus.AidNotFound;
            }
            if (_dbcontext.MemberAids.Any(m => m.AidID == aidID && m.MemberID == member.MemberID))
            {
                return AidStatus.AlreadyTook;
            }
            MemberAid memberAid = new MemberAid();
            memberAid.AidID = aidID;
            memberAid.ServantID = servant.ServantID;
            memberAid.MemberID = member.MemberID;
            memberAid.TimeStamp = CurrentTime;
            memberAid.Notes = notes;

            aid.ActualMembersCount++;
            _dbcontext.MemberAids.Add(memberAid);
            _dbcontext.SaveChanges();
            return AidStatus.OK;
        }
        public List<Member> GetAidMembers(int aidID)
        {
            List<Member> members = _dbcontext.MemberAids.Where(er => er.AidID == aidID ).OrderByDescending(ev => ev.TimeStamp).Select(er => er.Member).ToList();
            return members;
        }
        public List<Aid> GetAids(string username)
        {
            var servant = GetServantByUsername(username);
            if (servant == null)
                throw new Exception("Servant not found");
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servant.ServantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> aids = _dbcontext.AidClasses.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.AidID).ToList<int>();
            return _dbcontext.Aids.Where(e => aids.Contains(e.AidID) && e.IsActive).ToList();
        }
    }
}
