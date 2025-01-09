namespace SM.DAL
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using SM.DAL.DataModel;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext() {  }

        public DbSet<Member> Members { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassMember> ClassMembers { get; set; }
        public DbSet<ClassOccurrence> ClassOccurrences { get; set; }
        public DbSet<Servant> Servants { get; set; }
        public DbSet<ServantClass> ServantClasses { get; set; }
        public DbSet<ClassAttendance> ClassAttendances { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ClassEvent> ClassEvents { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<EventAttendance> EventAttendances { get; set; }
        public DbSet<Aid> Aids { get; set; }
        public DbSet<MemberAid> MemberAids { get; set; }
        public DbSet<Fund> Funds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_connectionString, DBVersion);
            }
            /*optionsBuilder.UseMySql("Server=your_server;Database=your_database;User=your_username;Password=your_password;",
                new MySqlServerVersion(new Version(8, 0, 2))); // Adjust for your MySQL version*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassMember>()
                .HasKey(cm => new { cm.ClassID, cm.MemberID });

            modelBuilder.Entity<ClassAttendance>()
                .HasKey(cm => new { cm.ClassOccurrenceID, cm.MemberID });

            modelBuilder.Entity<ServantClass>()
                .HasKey(sc => new { sc.ClassID, sc.ServantID });

            modelBuilder.Entity<ClassEvent>()
                .HasKey(ce => new { ce.ClassID, ce.EventID });

            modelBuilder.Entity<EventRegistration>()
                .HasKey(er => new { er.EventID, er.MemberID });

            modelBuilder.Entity<EventAttendance>()
                .HasKey(ea => new { ea.EventID, ea.MemberID });

            modelBuilder.Entity<MemberAid>()
                .HasKey(ea => new { ea.AidID, ea.MemberID });
        }

        static MySqlServerVersion _serverVersion;
        static string _connectionString
        {
            get; set;
        }
        public static MySqlServerVersion DBVersion
        {
            get
            {
                _serverVersion = new MySqlServerVersion(new Version(8, 0, 40));
                return _serverVersion;
            }
        }

        public static void ConfigureDbContextOptions(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            _connectionString = connectionString;
            optionsBuilder.UseMySql(connectionString, DBVersion);
        }
    }

}
