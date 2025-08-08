using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace SM.APP.Services
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenRefreshMiddleware> _logger;
        // Define a grace period before the token actually expires to refresh it
        private static readonly TimeSpan TokenRefreshGracePeriod = TimeSpan.FromMinutes(5);

        public TokenRefreshMiddleware(RequestDelegate next, ILogger<TokenRefreshMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser> userManager)
        {
            // First, let the rest of the pipeline run to establish the user's identity
            await _next(context);

            // Only proceed if the request is authenticated and the response hasn't started
            if (context.User.Identity?.IsAuthenticated == true && !context.Response.HasStarted)
            {
                var tokenCookie = context.Request.Cookies["AuthToken"];

                // Check if the token needs refreshing (either missing or expiring soon)
                if (ShouldRefreshToken(tokenCookie))
                {
                    try
                    {
                        var user = await userManager.GetUserAsync(context.User);
                        if (user != null)
                        {
                            var roles = await userManager.GetRolesAsync(user);
                            var expirationTime = DateTime.UtcNow.AddMinutes(SMConfigurationManager.TokenExpiry);

                            // Use your existing service to generate a new token
                            var newToken = AuthenticatorService.GenerateToken(user, roles, expirationTime);

                            // Get the existing cookie options from your login page logic
                            var cookieOptions = GetCookieOptions(expirationTime, context.Request.Host.Host);

                            // Set the new token in the response cookie, overwriting the old one
                            context.Response.Cookies.Append("AuthToken", newToken, cookieOptions);
                            _logger.LogInformation("AuthToken was refreshed for user {UserId}", user.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while trying to refresh the AuthToken.");
                    }
                }
            }
        }

        private bool ShouldRefreshToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return true; // No token, definitely should create one.
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var expiryTime = jwtToken.ValidTo;

                // Refresh if the token is already expired or within the grace period
                return DateTime.UtcNow >= expiryTime - TokenRefreshGracePeriod;
            }
            catch
            {
                return true; // Token is malformed or invalid, so refresh it.
            }
        }

        private CookieOptions GetCookieOptions(DateTime expires, string host)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Always true for production
                SameSite = SameSiteMode.None, // Required for cross-site API calls
                Expires = expires,
                Path = "/"
            };


            if (!SMConfigurationManager.IsDevelopment)
            {
                options.Domain = SMConfigurationManager.Domian;
            }

            return options;
        }
    }
}
