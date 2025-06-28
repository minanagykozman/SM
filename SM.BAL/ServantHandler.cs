using Microsoft.AspNetCore.Identity;
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
    public class ServantHandler : HandlerBase
    {
        public int? GetServantID(string userID)
        {
            var servant = _dbcontext.Servants.FirstOrDefault(s => s.UserID == userID);

            if (servant == null)
            {
                return null;
            }
            return servant.ServantID;
        }
        public Servant GetServantByUsername(string username)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
                return null;
            return _dbcontext.Servants.FirstOrDefault(s => s.UserID == user.Id);
        }
        public bool CheckEmail(string email)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserName == email);
            if (user == null)
                return true;
            return false;
        }
        public Servant CreateServant(string name, string mobile1, string mobile2, string id, List<int> classes)
        {
            Servant servant = new Servant()
            {
                ServantName = name,
                Mobile1 = mobile1,
                Mobile2 = mobile2,
                UserID = id,
                IsActive = true
            };
            _dbcontext.Servants.Add(servant);
            _dbcontext.SaveChanges();

            foreach (var item in classes)
            {
                _dbcontext.ServantClasses.Add(new ServantClass() { ClassID = item, ServantID = servant.ServantID });
            }
            _dbcontext.SaveChanges();
            return servant;
        }
    }
}
