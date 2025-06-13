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
        public int CreateAid(Aid aid, List<int> classIDs)
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
            foreach (int classID in classIDs)
            {
                _dbcontext.AidClasses.Add(new AidClass() { ClassID = classID, AidID = aidNew.AidID });
            }

            _dbcontext.SaveChanges();
            return aidNew.AidID;
        }
        public int UpdateAid(Aid aidToUpdate, List<int> classIDs)
        {
            try
            {
                Aid? ev = _dbcontext.Aids.Where(e => e.AidID == aidToUpdate.AidID).FirstOrDefault();
                if (ev == null)
                    throw new Exception("Aid not found");

                ev.AidDate = aidToUpdate.AidDate;
                ev.AidName = aidToUpdate.AidName;
                ev.CostPerPerson = aidToUpdate.CostPerPerson;
                ev.Components = aidToUpdate.Components;
                ev.PlannedMembersCount = aidToUpdate.PlannedMembersCount;
                ev.IsActive = aidToUpdate.IsActive; ;
                ev.TotalCost = aidToUpdate.TotalCost;

                var classAids= _dbcontext.AidClasses.Where(e => e.AidID == aidToUpdate.AidID).ToList();
                _dbcontext.AidClasses.RemoveRange(classAids);
                _dbcontext.SaveChanges();

                foreach (int classID in classIDs)
                {
                    _dbcontext.AidClasses.Add(new AidClass() { ClassID = classID, AidID = ev.AidID});
                }
                _dbcontext.SaveChanges();
                return ev.AidID;
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
                return 0;
            }
        }
        public int DeleteAid(int aidID)
        {
            try
            {
                var ev = _dbcontext.Aids.Where(e => e.AidID == aidID).FirstOrDefault();
                if (ev == null)
                    throw new Exception("Event not found");
                ev.IsDeleted = true;

                _dbcontext.SaveChanges();
                return ev.AidID;
            }
            catch (Exception ex)
            {
                Logger.log.Error("", ex);
                return 0;
            }
        }
        public Aid? GetAid(int aidID)
        {
            var ev = _dbcontext.Aids.Include(e => e.AidClasses).Where(e => e.AidID == aidID).FirstOrDefault();
            return ev;
        }
        public AidStatus CheckMemberStatus(int aidID, string memberCode, out Member member)
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
        public List<MemberAid> GetAidMembers(int aidID)
        {
            List<MemberAid> members = _dbcontext.MemberAids.Include(ma => ma.Member).Where(er => er.AidID == aidID).OrderByDescending(ev => ev.TimeStamp).ToList();
            return members;
        }
        public List<Aid> GetAids(string username)
        {
            var servant = GetServantByUsername(username);
            if (servant == null)
                throw new Exception("Servant not found");
            List<int> classes = _dbcontext.ServantClasses.Where(sc => sc.ServantID == servant.ServantID).Select(sc => sc.ClassID).ToList<int>();
            List<int> aids = _dbcontext.AidClasses.Where(ce => classes.Contains(ce.ClassID)).Select(ce => ce.AidID).ToList<int>();
            return _dbcontext.Aids.Where(e => aids.Contains(e.AidID) && e.IsActive).OrderBy(a => a.AidDate).ToList();
        }
    }
}
