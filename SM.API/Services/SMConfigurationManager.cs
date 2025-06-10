using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SM.API.Services
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
            get { return 120; }
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

        public static string S3BucketName
        {
            get
            {
                if (IsDevelopment)
                    return _configuration["S3Name"] ?? string.Empty;
                else
                    return Environment.GetEnvironmentVariable("S3Name") ?? string.Empty;
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
