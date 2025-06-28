using Amazon.S3;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SM.API.Services;
using SM.DAL;
using System.Text;
using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
// Pass IConfiguration to the helper class
SMConfigurationManager.SetConfiguration(builder.Configuration);
// Retrieve the connection string from appsettings.json
string connectionString = string.Empty;
string loggerConnectionString = string.Empty;
string hangfireConnectionString = string.Empty;
string secretKey = string.Empty;
string issuer = string.Empty;
string audience = string.Empty;
if (!builder.Environment.IsDevelopment())
{
    connectionString = Environment.GetEnvironmentVariable("DBConnectionString")!;
    loggerConnectionString = Environment.GetEnvironmentVariable("LoggerDB")!;
    hangfireConnectionString = Environment.GetEnvironmentVariable("HangFireDB")!;
    secretKey = Environment.GetEnvironmentVariable("JWTSecretKey")!;
    issuer = Environment.GetEnvironmentVariable("JWTIssuer")!;
    audience = Environment.GetEnvironmentVariable("JWTAudience")!;
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(5000); // HTTP (Only for internal communication with Nginx)
    });
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DBConnectionString")!;
    loggerConnectionString = builder.Configuration.GetConnectionString("LoggerDB")!;
    hangfireConnectionString = builder.Configuration.GetConnectionString("HangFireDB")!;
    secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
    issuer = builder.Configuration["JwtSettings:Issuer"]!;
    audience = builder.Configuration["JwtSettings:Audience"]!;
}

// Register ApplicationDbContext with the MySQL connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    AppDbContext.ConfigureDbContextOptions(options, connectionString));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    // For any authentication, use JWTs.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // If an unauthenticated user tries to access a protected endpoint,
    // challenge them with the JWT scheme, which returns a 401 Unauthorized.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    // Use this scheme for any other default behavior.
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // This configures the JWT handler itself
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    // Your cookie retrieval logic remains perfectly valid
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        }
    };
});


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
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.Console()
    .WriteTo.MySQL(
        connectionString: loggerConnectionString,
        tableName: "APILogs"
    )
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddAWSService<IAmazonS3>();
// Add Swagger with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Enter 'Bearer <your_token>'",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHangfire(
    configuration => configuration.UseStorage(new MySqlStorage(hangfireConnectionString, new MySqlStorageOptions
    {
        TablesPrefix = "Hangfire"
    })));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<Jobs>();
builder.Services.AddHostedService<JobScheduler>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100 MB
});
var app = builder.Build();

// Automatically apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}



app.UseSwagger();
app.UseSwaggerUI();

//Apply CORS 
app.UseCors("AllowAppAndApi");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard("/hangfire");

app.Run();
