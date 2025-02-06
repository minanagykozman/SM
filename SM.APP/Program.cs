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
        // Retrieve the connection string from appsettings.json
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Register ApplicationDbContext with the MySQL connection string
        builder.Services.AddDbContext<AppDbContext>(options =>
            AppDbContext.ConfigureDbContextOptions(options, connectionString));

        // Add services to the container.
        builder.Services.AddRazorPages();


        if (!builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(5000); // HTTP (Only for internal communication with Nginx)
            });
        }

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

        //builder.Services.AddSingleton<IServantManager, ServantManager>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        //if (!app.Environment.IsDevelopment())
        //{
        //    app.UseExceptionHandler("/Error");
        //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //    app.UseHsts();
        //}

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}