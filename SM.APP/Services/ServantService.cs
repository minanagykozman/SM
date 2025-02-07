using Microsoft.AspNetCore.Identity;
using SM.BAL;
using SM.DAL;
using SM.DAL.DataModel;
using SQLitePCL;
using System.Numerics;
using System.Security.Claims;

namespace SM.APP.Services
{
    public class ServantService
    {
        

        public int GetServantID(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                using (ServantHandler handler = new ServantHandler())
                {
                    var servantID= handler.GetServantID(userId);
                    if(servantID.HasValue)
                        return servantID.Value;
                    else return 0;
                }
            }
            return 0;
        }

    }
}
