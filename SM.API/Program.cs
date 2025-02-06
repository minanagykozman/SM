using Microsoft.EntityFrameworkCore;
using SM.DAL;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register ApplicationDbContext with the MySQL connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    AppDbContext.ConfigureDbContextOptions(options, connectionString));


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(5000); // HTTP (Only for internal communication with Nginx)
    });
}

var app = builder.Build();

// Automatically apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
