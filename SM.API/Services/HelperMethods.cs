using System.Security.Claims;

namespace SM.API.Services
{
    public static class HelperMethods
    {
        public static void ValidateUser(ClaimsPrincipal user)
        {
            if (user == null)
                throw new Exception("User not logged in, unable to retrieve Servant details");
            if (user.Identity == null)
                throw new Exception("User not logged in, unable to retrieve Servant details");
            if (string.IsNullOrEmpty(user.Identity.Name))
                throw new Exception("User not logged in, unable to retrieve Servant details");
        }
    }
}
