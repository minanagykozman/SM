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
