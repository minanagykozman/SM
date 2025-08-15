using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SM.APP.Services;
using SM.DAL;
using Serilog;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authorization;
using SM.BAL.Services.Authorization;
using SM.BAL;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Pass IConfiguration to the helper class
        SMConfigurationManager.SetConfiguration(builder.Configuration);

        // Add services to the container.
        builder.Services.AddRazorPages();

  
        if (!builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(5000); 
            });
        }
        
        // Register ApplicationDbContext with the MySQL connection string
        builder.Services.AddDbContext<AppDbContext>(options =>
            AppDbContext.ConfigureDbContextOptions(options, SMConfigurationManager.DBConnection));


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
        builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddAuthorization(options =>
        {
            // This is a common pattern to resolve services during startup
            // to fetch data from the database.
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            using (PermissionHandler permissionService = new PermissionHandler())
            {

                // Get all distinct permission names from the database
                var allPermissions = permissionService.GetAllPermissionNamesAsync().GetAwaiter().GetResult();

                foreach (var permissionName in allPermissions)
                {
                    // For each permission, create a new policy
                    options.AddPolicy(permissionName, policy =>
                        policy.AddRequirements(new PermissionRequirement(permissionName)));
                }
            }
        });


        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
        });
        builder.Services.AddSingleton<IEmailSender, EmailSender>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAppAndApi",
                policy =>
                {
                    policy.WithOrigins(SMConfigurationManager.JWTIssuer)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });
        builder.Services.AddSession(); // Add session service
        builder.Services.AddDistributedMemoryCache(); // Required for session storage

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Console()
            .WriteTo.MySQL(
                connectionString: SMConfigurationManager.LogDBConnection,
                tableName: "Logs"
            )
            .CreateLogger();
        builder.Host.UseSerilog();
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //dbContext.Database.Migrate();
        }

        //app.Use(async (context, next) =>
        //{

        //    context.Response.Headers.Add("Content-Security-Policy",
        //        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self';");
        //    await next();
        //});

        app.UseCors("AllowAppAndApi");

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TokenRefreshMiddleware>();

        app.MapRazorPages();

        app.Run();
    }
}