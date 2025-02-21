using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.BAL
{
    public class MemberHandler : HandlerBase
    {
        public List<Member> GetFamilyByUNFileNumber(string unFileNumber)
        {
            var members = _dbcontext.Members.Where(m => m.UNFileNumber == unFileNumber).ToList<Member>();
            return members.OrderBy(m => m.Birthdate).ToList<Member>();
        }

        public Member? GetMember(string memberCode)
        {
            return _dbcontext.Members.
                FirstOrDefault(m => m.Code.Contains(memberCode)
                || m.UNPersonalNumber == memberCode
                || (m.UNFileNumber == memberCode && m.IsMainMember));
        }
        public Member? GetMemberByCodeOnly(string memberCode)
        {
            return _dbcontext.Members.FirstOrDefault(m => m.Code.Contains(memberCode));
        }

        public Member GetMember(int memberID)
        {
            return _dbcontext.Members.FirstOrDefault(m => m.MemberID == memberID);
        }
        public void UpdateMember(Member member)
        {
            _dbcontext.Attach(member).State = EntityState.Modified;

            try
            {
                _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;
            }
        }
        public List<Member> GetMembersByCardStatus(CardStatus status)
        {
            return _dbcontext.Members.Where(m => m.CardStatus.ToLower() == status.ToString().ToLower()).OrderByDescending(m => m.LastModifiedDate).ToList();
        }
        public List<Member> GetMembers(string memberCode, string firstName, string lastName)
        {
            List<Member> members = new List<Member>();
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                var member = _dbcontext.Members.Include(m => m.ClassMembers).
                    FirstOrDefault(m => m.Code == memberCode ||
                    m.UNPersonalNumber == memberCode || m.UNFileNumber == memberCode);

                if (member == null)
                    return members;
                if (string.IsNullOrEmpty(member.UNFileNumber))
                {
                    members.Add(member);
                    return members;
                }
                members = _dbcontext.Members.Where(m => m.UNFileNumber == member.UNFileNumber).OrderBy(m => m.Birthdate).ToList();
            }
            else
            {
                if (string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    members = _dbcontext.Members.Where(m => m.UNLastName.Contains(lastName)).ToList();
                }
                else if (!string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                {
                    members = _dbcontext.Members.Where(m => m.UNFirstName.Contains(firstName)).ToList();
                }
                else
                {
                    members = _dbcontext.Members.Where(m => m.UNFirstName.Contains(firstName) && m.UNLastName.Contains(lastName)).ToList();
                }
            }
            return members;

        }

        public string GenerateCode(string gender, DateTime birthdate)
        {
            int seq = _dbcontext.Members.Select(m => m.Sequence).Max() + 1;
            return string.Format("{0}{1}-{2}", birthdate.ToString("yy"), gender, seq.ToString("0000"));
        }
        public bool ValidateUNNumber(string unPersonalNo)
        {
            return !_dbcontext.Members.Any(m => m.UNPersonalNumber.ToLower() == unPersonalNo.ToLower());
        }

        public bool UpdateCardStatus(string memberCode, CardStatus cardStatus)
        {
            var member = GetMemberByCodeOnly(memberCode);
            if (member == null)
            {
                return false;
            }
            member.CardStatus = cardStatus.ToString();
            member.LastModifiedDate = CurrentTime;
            _dbcontext.SaveChanges();
            return true;
        }
    }
}
