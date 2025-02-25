﻿using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.BAL
{
    public class ServantHandler : IDisposable
    {
        private AppDbContext _dbcontext;
        public ServantHandler()
        {
            _dbcontext = new AppDbContext();
        }



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
        public void Dispose()
        {
            if (_dbcontext != null)
                _dbcontext.Dispose();
        }
    }
}
