using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SM.APP.Services
{
    public static class SMConfigurationManager
    {
        private static IConfiguration _configuration = default!;

        public static void SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public static int TokenExpiry
        {
            get
            {
                int tokenExpiry = 0;
                if (IsDevelopment)
                    int.TryParse(_configuration["JwtSettings:TokenExpiry"], out tokenExpiry);
                else
                    int.TryParse(Environment.GetEnvironmentVariable("TokenExpiry"), out tokenExpiry);
                return tokenExpiry;
            }
        }
        public static string ApiBase
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["AppSettings:API"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("ApiBase") ?? string.Empty;
            }
        }
        public static string Domian
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["AppSettings:Domian"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("Domian") ?? string.Empty;
            }
        }
        public static string JWTSecret
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["JwtSettings:SecretKey"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("JWTSecretKey") ?? string.Empty;
            }
        }
        public static string JWTAudience
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["JwtSettings:Audience"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("JWTAudience") ?? string.Empty;
            }
        }
        public static string JWTIssuer
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["JwtSettings:Issuer"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("JWTIssuer") ?? string.Empty;
            }
        }
        public static string DBConnection
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["ConnectionStrings:DBConnectionString"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("DBConnectionString") ?? string.Empty;
            }
        }
        public static string LogDBConnection
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["ConnectionStrings:LoggerDB"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("LoggerDB") ?? string.Empty;
            }
        }
        public static bool IsDevelopment
        {
            get
            {
                string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
                return env == "Development";
            }
        }
    }
}
