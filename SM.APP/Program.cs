using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SM.APP.Services;
using SM.DAL;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Pass IConfiguration to the helper class
        SMConfigurationManager.SetConfiguration(builder.Configuration);

        // Add services to the container.
        builder.Services.AddRazorPages();

        string connectionString = string.Empty;
        string secretKey = string.Empty;
        string issuer = string.Empty;
        string audience = string.Empty;
        if (!builder.Environment.IsDevelopment())
        {
            connectionString = Environment.GetEnvironmentVariable("DBConnectionString");
            secretKey = Environment.GetEnvironmentVariable("DBConnectionString");
            issuer = Environment.GetEnvironmentVariable("DBConnectionString");
            audience = Environment.GetEnvironmentVariable("DBConnectionString");
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(5000); // HTTP (Only for internal communication with Nginx)
            });
        }
        else
        {
            connectionString = builder.Configuration.GetConnectionString("DBConnectionString");
            secretKey = builder.Configuration["JwtSettings:SecretKey"];
            issuer = builder.Configuration["JwtSettings:Issuer"];
            audience = builder.Configuration["JwtSettings:Audience"];
        }
        // Register ApplicationDbContext with the MySQL connection string
        builder.Services.AddDbContext<AppDbContext>(options =>
            AppDbContext.ConfigureDbContextOptions(options, connectionString));


        // Add Identity with Role Support
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        });

        builder.Services.AddSingleton<IEmailSender, EmailSender>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAppAndApi",
                policy =>
                {
                    policy.WithOrigins(issuer, audience)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });
        builder.Services.AddSession(); // Add session service
        builder.Services.AddDistributedMemoryCache(); // Required for session storage

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }


        app.UseCors("AllowAppAndApi");

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}