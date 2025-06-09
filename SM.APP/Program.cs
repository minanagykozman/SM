using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SM.APP.Services;
using SM.DAL;
using Serilog;
using Microsoft.AspNetCore.Http.Features;
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
        string loggerConnectionString = string.Empty;
        string secretKey = string.Empty;
        string issuer = string.Empty;
        string audience = string.Empty;
        if (!builder.Environment.IsDevelopment())
        {
            connectionString = Environment.GetEnvironmentVariable("DBConnectionString")!;
            loggerConnectionString = Environment.GetEnvironmentVariable("LoggerDB")!;
            secretKey = Environment.GetEnvironmentVariable("JWTSecretKey")!;
            issuer = Environment.GetEnvironmentVariable("JWTIssuer")!;
            audience = Environment.GetEnvironmentVariable("JWTAudience")!;
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(5000); 
            });
        }
        else
        {
            connectionString = builder.Configuration.GetConnectionString("DBConnectionString")!;
            loggerConnectionString = builder.Configuration.GetConnectionString("LoggerDB")!;
            secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
            issuer = builder.Configuration["JwtSettings:Issuer"]!;
            audience = builder.Configuration["JwtSettings:Audience"]!;
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
                    policy.WithOrigins(issuer)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });
        builder.Services.AddSession(); // Add session service
        builder.Services.AddDistributedMemoryCache(); // Required for session storage
        //Serilog.Debugging.SelfLog.Enable(msg =>
        //{
        //    System.IO.File.AppendAllText("C:\\Users\\Administrator\\Documents\\Mina\\Evangalism\\Apps\\AWS\\ssudan.stpaul\\serilog-errors.txt", msg + Environment.NewLine);
        //});

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Console()
            .WriteTo.MySQL(
                connectionString: loggerConnectionString,
                tableName: "Logs"
            )
            .CreateLogger();
        builder.Host.UseSerilog();
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
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

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}