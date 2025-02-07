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
            return  _dbcontext.Members.Include(m => m.ClassMembers).
                FirstOrDefault(m => m.Code == memberCode || m.UNPersonalNumber == memberCode || (m.UNFileNumber == memberCode && m.IsMainMember));
        }
        public void Dispose()
        {
            if (_dbcontext != null)
                _dbcontext.Dispose();
        }
    }
}
