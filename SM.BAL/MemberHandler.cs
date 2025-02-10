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
    public class MemberHandler : IDisposable
    {
        private AppDbContext _dbcontext;
        public MemberHandler()
        {
            _dbcontext = new AppDbContext();
        }

        public List<Member> GetFamilyByUNFileNumber(string unFileNumber)
        {
            var members = _dbcontext.Members.Where(m => m.UNFileNumber == unFileNumber).ToList<Member>();
            return members.OrderBy(m => m.Birthdate).ToList<Member>();
        }

        public Member? GetMember(string memberCode)
        {
            return _dbcontext.Members.Include(m => m.ClassMembers).
                FirstOrDefault(m => m.Code == memberCode || m.UNPersonalNumber == memberCode || (m.UNFileNumber == memberCode && m.IsMainMember));
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
            return string.Format("{0}{1}-{2}");
        }
        public bool IsUNPersonalNoUnique(string unPersonalNo)
        {
            return !_dbcontext.Members.Any(m => m.UNPersonalNumber.ToLower() == unPersonalNo.ToLower());
        }
        public void Dispose()
        {
            if (_dbcontext != null)
                _dbcontext.Dispose();
        }
    }
}
