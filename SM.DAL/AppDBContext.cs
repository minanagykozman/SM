namespace SM.DAL
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using SM.DAL.DataModel;

    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext() { }

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
        public DbSet<Aid> Aids { get; set; }
        public DbSet<MemberAid> MemberAids { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<MemberEventView> MemberEventView { get; set; }
        public DbSet<MemberClasssAttendanceView> MemberClasssAttendanceView { get; set; }
        public DbSet<AidClass> AidClasses { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<MemberFund> MemberFunds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_connectionString, DBVersion, mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Visitation>()
                .Property(u => u.VisitationID).ValueGeneratedOnAdd();
            modelBuilder.Entity<MemberFund>().HasKey(u => u.FundID);
            modelBuilder.Entity<MemberFund>()
                .Property(u => u.FundID).ValueGeneratedOnAdd();

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
            modelBuilder.Entity<AidClass>()
                .HasKey(ac => new { ac.AidID, ac.ClassID });


            modelBuilder.Entity<MemberAid>()
                .HasKey(ea => new { ea.AidID, ea.MemberID });
            modelBuilder.Entity<Member>()
            .HasIndex(m => m.Code).IsUnique();
            modelBuilder.Entity<Member>()
            .HasIndex(m => m.UNPersonalNumber).IsUnique();
            modelBuilder.Entity<Member>()
            .HasIndex(m => m.Sequence).IsUnique();

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            modelBuilder.Entity<MemberEventView>().ToView("MemberEventView").HasKey(v => new { v.MemberID, v.EventID });
            modelBuilder.Entity<MemberClasssAttendanceView>().ToView("MemberClasssAttendanceView").HasKey(v => new { v.MemberID, v.ClassOccurrenceID });

            #region Seed Data
            // Seed roles

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "383d20f8-ac3b-46ff-b322-4f21cda036dc", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "30f10ff2-8ac3-4acc-b88d-abb2fd554653", Name = "Servant", NormalizedName = "SERVANT" }
            );

            // Seed an Admin user
            var hasher = new PasswordHasher<IdentityUser>();

            var adminUser = new IdentityUser
            {
                Id = "78769c53-5336-4411-8488-1983111d7be7",
                UserName = "ssudan.stpual@gmail.com",
                NormalizedUserName = "SSUDAN.STPAUL@GMAIL.COM",
                Email = "ssudan.stpual@gmail.com",
                NormalizedEmail = "SSUDAN.STPAUL@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEAvxm8Ul6/CAsvy/Ylk9GobRdQrfCnhyTSTEO0s149pYHw6oPn9vcCqhwIvF558hSw==",
                SecurityStamp = "394139b6-fd9c-4d25-a9a5-ef1565d13bab",
                ConcurrencyStamp = "7c62d744-b320-49c6-871a-273d2f53a81f"
            };
            //hasher.HashPassword(null, "stP@ul$25")
            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            // Assign the Admin user to the Admin role
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "78769c53-5336-4411-8488-1983111d7be7", RoleId = "383d20f8-ac3b-46ff-b322-4f21cda036dc" }
            );
            var adminServant = new Servant()
            {
                ServantID = -1,
                ServantName = "admin",
                IsActive = true,
                UserID = "78769c53-5336-4411-8488-1983111d7be7"
            };
            modelBuilder.Entity<Servant>().HasData(adminServant);
            #endregion
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
            optionsBuilder.UseMySql(connectionString, DBVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        }
    }

}
