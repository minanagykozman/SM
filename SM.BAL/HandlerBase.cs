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
    public class HandlerBase : IDisposable
    {
        internal AppDbContext _dbcontext;
        public HandlerBase()
        {
            _dbcontext = new AppDbContext();
        }
        public Servant GetServantByUsername(string username)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
                return null;
            return _dbcontext.Servants.FirstOrDefault(s => s.UserID == user.Id);
        }
        public Member? GetMember(string memberCode)
        {
            memberCode = memberCode.Trim();
            return _dbcontext.Members.
                FirstOrDefault(m => m.Code.Contains(memberCode)
                || m.UNPersonalNumber == memberCode
                || (m.UNFileNumber == memberCode && m.IsMainMember));
        }
        public Member? GetMemberWithClasses(string memberCode)
        {
            memberCode = memberCode.Trim();
            return _dbcontext.Members.Include(m => m.ClassMembers).
                FirstOrDefault(m => m.Code.Contains(memberCode)
                || m.UNPersonalNumber == memberCode
                || (m.UNFileNumber == memberCode && m.IsMainMember));
        }
        public bool IsAdmin(string username)
        {
            if (string.IsNullOrEmpty(username)) return false;

            var normalizedUsername = username.ToUpperInvariant();
            
            var isAdmin = _dbcontext.Users
                .Where(u => u.NormalizedUserName == normalizedUsername)
                .Join(_dbcontext.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => ur)
                .Join(_dbcontext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.NormalizedName)
                .Any(roleName => roleName == "ADMIN");

            return isAdmin;
        }

        public DateTime CurrentTime
        {
            get
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); // Example for UTC+2
                DateTime utcNow = DateTime.UtcNow;
                DateTime utcPlus2 = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
                return utcPlus2;
            }
        }
        public void Dispose()
        {
            if (_dbcontext != null)
                _dbcontext.Dispose();
        }
    }
}
