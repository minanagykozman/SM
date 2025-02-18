using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SM.APP.Services
{
    public static class SMConfigurationManager
    {
        private static IConfiguration _configuration;

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static string ApiBase
        {
            get
            {
                if (_isDevelopment)
                    return _configuration["AppSettings:API"] ?? string.Empty; 
                else
                    return Environment.GetEnvironmentVariable("ApiBase") ?? string.Empty;
            }
        }
        public static string JWTSecret
        {
            get
            {
                if (_isDevelopment)
                    return _configuration["JwtSettings:SecretKey"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("JWTSecretKey") ?? string.Empty;
            }
        }
        public static string JWTAudience
        {
            get
            {
                if (_isDevelopment)
                    return _configuration["JwtSettings:Audience"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("JWTAudience") ?? string.Empty;
            }
        }
        public static string JWTIssuer
        {
            get
            {
                if (_isDevelopment)
                    return _configuration["JwtSettings:Issuer"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("JWTIssuer") ?? string.Empty;
            }
        }
        static bool _isDevelopment

        {
            get
            {
                string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
                return env == "Development";
            }
        }
    }
}
