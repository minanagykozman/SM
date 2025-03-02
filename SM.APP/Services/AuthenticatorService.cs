using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SM.APP.Services
{
    public class AuthenticatorService
    {
        public static string GenerateToken(IdentityUser user, IList<string> roles,DateTime expirationTime)
        {
            var key = Encoding.UTF8.GetBytes(SMConfigurationManager.JWTSecret);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = SMConfigurationManager.JWTIssuer,
                Audience = SMConfigurationManager.JWTAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SMConfigurationManager.JWTSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // Change to `true` if verifying issuer
                ValidateAudience = false, // Change to `true` if verifying audience
                ValidateLifetime = true, // Checks expiration time
                ClockSkew = TimeSpan.Zero // No leeway for token expiry
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true; // Token is valid
            }
            catch
            {
                return false; // Invalid token (expired, tampered, or signature mismatch)
            }
        }
        
    }
}
