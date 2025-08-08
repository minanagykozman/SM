using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.BAL
{
    public class PermissionHandler : HandlerBase
    {
        public async Task<IEnumerable<string>> GetAllPermissionNamesAsync()
        {
            return await _dbcontext.Permissions
                                   .Select(p => p.Name)
                                   .Distinct()
                                   .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(IEnumerable<string> userRoles, string permissionName)
        {
            if (!userRoles.Any())
            {
                return false;
            }

            return await _dbcontext.RolePermissions
                .AnyAsync(rp => userRoles.Contains(rp.Role.Name) &&
                                 rp.Permission.Name == permissionName);
        }

    }
}
