using Microsoft.EntityFrameworkCore;
using SM.DAL;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from appsettings.json
string connectionString = string.Empty;
if (!builder.Environment.IsDevelopment())
{
    connectionString = Environment.GetEnvironmentVariable("DBConnectionString");
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(5000); // HTTP (Only for internal communication with Nginx)
    });
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DBConnectionString");
}

// Register ApplicationDbContext with the MySQL connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    AppDbContext.ConfigureDbContextOptions(options, connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin() //  Allows any frontend
                        .AllowAnyMethod() //  Allows GET, POST, PUT, DELETE, etc.
                        .AllowAnyHeader()); //  Allows any headers
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Automatically apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


app.UseSwagger();
app.UseSwaggerUI();

//Apply CORS globally
app.UseCors("AllowAll");



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
