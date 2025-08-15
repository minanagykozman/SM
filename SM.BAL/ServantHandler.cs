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
        public Servant CreateServant(string name, string mobile1, string mobile2, string id, int churchID, List<int> classes)
        {
            Servant servant = new Servant()
            {
                ServantName = name,
                Mobile1 = mobile1,
                Mobile2 = mobile2,
                UserID = id,
                IsActive = true,
                ChurchID = churchID
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

        public Servant UpdateServant(int servantID, string name, string mobile1, string mobile2, List<int> classes, List<string> roles)
        {
            Servant servant = _dbcontext.Servants.Where(s => s.ServantID == servantID).FirstOrDefault();
            if (servant == null)
            {
                throw new Exception("Servant not found!");
            }
            servant.ServantName = name;
            servant.Mobile1 = mobile1;
            servant.Mobile2 = mobile2;

            var oldClasses = _dbcontext.ServantClasses.Where(cm => cm.ServantID == servantID).ToList();
            var oldIds = new List<int>(oldClasses.Select(m => m.ClassID));
            if (classes == null)
            {
                foreach (var cl in oldClasses)
                {
                    _dbcontext.ServantClasses.Remove(cl);

                }
            }
            else
            {
                var toBeDeleted = oldClasses.Where(m => !classes!.Contains(m.ClassID)).ToList();
                var toBeAdded = classes.Where(m => !oldIds.Contains(m)).ToList();

                foreach (var cl in toBeAdded)
                {
                    _dbcontext.ServantClasses.Add(new ServantClass() { ServantID = servantID, ClassID = cl });
                }

                foreach (var cl in toBeDeleted!)
                {
                    _dbcontext.ServantClasses.Remove(cl);

                }
            }


            var oldRoles = _dbcontext.UserRoles.Where(cm => cm.UserId == servant.UserID).ToList();
            var oldRolesIds = new List<string>(oldRoles.Select(m => m.RoleId));
            if (roles == null)
            {
                foreach (var r in oldRoles)
                {
                    _dbcontext.UserRoles.Remove(r);

                }
            }
            else
            {
                var rolesToBeDeleted = oldRoles.Where(m => !roles!.Contains(m.RoleId)).ToList();
                var rolesToBeAdded = roles.Where(m => !oldRolesIds.Contains(m)).ToList();

                foreach (var r in rolesToBeAdded)
                {
                    _dbcontext.UserRoles.Add(new IdentityUserRole<string>() { UserId = servant.UserID, RoleId = r });
                }

                foreach (var r in rolesToBeDeleted!)
                {
                    _dbcontext.UserRoles.Remove(r);
                }
            }

            _dbcontext.SaveChanges();
            return servant;
        }

        public List<Servant> GetServants(bool? isActive)
        {
            List<Servant> servants = new List<Servant>();

            if (isActive.HasValue)
                servants = _dbcontext.Servants.Where(s => s.IsActive).ToList();
            servants = _dbcontext.Servants.ToList();

            if (servants != null)
            {
                var users = _dbcontext.Users.ToList();
                foreach (var servant in servants)
                {
                    var user = users.Where(u => u.Id == servant.UserID).FirstOrDefault();
                    servant.Email = user?.UserName;
                }
            }

            return servants;
        }
        public Servant GetServant(int servantID)
        {
            var servant = _dbcontext.Servants.Include(s => s.ServantClasses).Where(s => s.ServantID == servantID).FirstOrDefault();
            var roleIDs = _dbcontext.UserRoles.Where(u => u.UserId == servant.UserID).Select(s => s.RoleId).ToList();
            string email = _dbcontext.Users.Where(u => u.Id == servant.UserID).FirstOrDefault().UserName;
            servant.Email = email;
            servant.ServantRoles = roleIDs;
            return servant;
        }
    }
}
