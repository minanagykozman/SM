﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SM.DAL;

#nullable disable

namespace SM.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250304140627_FixView")]
    partial class FixView
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = "383d20f8-ac3b-46ff-b322-4f21cda036dc",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "30f10ff2-8ac3-4acc-b88d-abb2fd554653",
                            Name = "Servant",
                            NormalizedName = "SERVANT"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = "78769c53-5336-4411-8488-1983111d7be7",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "7c62d744-b320-49c6-871a-273d2f53a81f",
                            Email = "ssudan.stpual@gmail.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "SSUDAN.STPAUL@GMAIL.COM",
                            NormalizedUserName = "SSUDAN.STPAUL@GMAIL.COM",
                            PasswordHash = "AQAAAAIAAYagAAAAEAvxm8Ul6/CAsvy/Ylk9GobRdQrfCnhyTSTEO0s149pYHw6oPn9vcCqhwIvF558hSw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "394139b6-fd9c-4d25-a9a5-ef1565d13bab",
                            TwoFactorEnabled = false,
                            UserName = "ssudan.stpual@gmail.com"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            UserId = "78769c53-5336-4411-8488-1983111d7be7",
                            RoleId = "383d20f8-ac3b-46ff-b322-4f21cda036dc"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Aid", b =>
                {
                    b.Property<int>("AidID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("AidID"));

                    b.Property<string>("AidName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("EventStartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("AidID");

                    b.ToTable("Aids");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Class", b =>
                {
                    b.Property<int>("ClassID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ClassID"));

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("MeetingID")
                        .HasColumnType("int");

                    b.HasKey("ClassID");

                    b.HasIndex("MeetingID");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassAttendance", b =>
                {
                    b.Property<int>("ClassOccurrenceID")
                        .HasColumnType("int");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ClassOccurrenceID", "MemberID");

                    b.HasIndex("MemberID");

                    b.HasIndex("ServantID");

                    b.ToTable("ClassAttendances");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassEvent", b =>
                {
                    b.Property<int>("ClassID")
                        .HasColumnType("int");

                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.HasKey("ClassID", "EventID");

                    b.HasIndex("EventID");

                    b.ToTable("ClassEvents");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassMember", b =>
                {
                    b.Property<int>("ClassID")
                        .HasColumnType("int");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.HasKey("ClassID", "MemberID");

                    b.HasIndex("MemberID");

                    b.ToTable("ClassMembers");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassOccurrence", b =>
                {
                    b.Property<int>("ClassOccurrenceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ClassOccurrenceID"));

                    b.Property<int>("ClassID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ClassOccurrenceEndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ClassOccurrenceStartDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ClassOccurrenceID");

                    b.HasIndex("ClassID");

                    b.ToTable("ClassOccurrences");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Event", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("EventID"));

                    b.Property<DateTime>("EventEndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("EventStartDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventID");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("SM.DAL.DataModel.EventRegistration", b =>
                {
                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<int?>("AttendanceServantID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("AttendanceTimeStamp")
                        .HasColumnType("datetime(6)");

                    b.Property<bool?>("Attended")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Bus")
                        .HasMaxLength(25)
                        .HasColumnType("varchar(25)");

                    b.Property<bool>("IsException")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

                    b.Property<string>("Team")
                        .HasMaxLength(25)
                        .HasColumnType("varchar(25)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventID", "MemberID");

                    b.HasIndex("MemberID");

                    b.HasIndex("ServantID");

                    b.ToTable("EventRegistrations");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Fund", b =>
                {
                    b.Property<int>("FundID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("FundID"));

                    b.Property<int>("AidID")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("FundID");

                    b.HasIndex("AidID");

                    b.HasIndex("MemberID");

                    b.HasIndex("ServantID");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Meeting", b =>
                {
                    b.Property<int>("MeetingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("MeetingID"));

                    b.Property<DateTime>("AgeEndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("AgeStartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(1)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("MeetingDay")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("MeetingEndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("MeetingEndTime")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MeetingFrequency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MeetingName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("MeetingStartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("MeetingStartTime")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.HasKey("MeetingID");

                    b.ToTable("Meetings");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Member", b =>
                {
                    b.Property<int>("MemberID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("MemberID"));

                    b.Property<bool>("Baptised")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CardStatus")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(1)");

                    b.Property<string>("ImageReference")
                        .HasColumnType("longtext");

                    b.Property<string>("ImageURL")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsMainMember")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Mobile")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedLog")
                        .HasColumnType("longtext");

                    b.Property<string>("Nickname")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<string>("School")
                        .HasColumnType("longtext");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.Property<string>("UNFileNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UNFirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("UNLastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UNPersonalNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Work")
                        .HasColumnType("longtext");

                    b.HasKey("MemberID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("Sequence")
                        .IsUnique();

                    b.HasIndex("UNPersonalNumber")
                        .IsUnique();

                    b.ToTable("Members");
                });

            modelBuilder.Entity("SM.DAL.DataModel.MemberAid", b =>
                {
                    b.Property<int>("AidID")
                        .HasColumnType("int");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("AidID", "MemberID");

                    b.HasIndex("MemberID");

                    b.HasIndex("ServantID");

                    b.ToTable("MemberAids");
                });

            modelBuilder.Entity("SM.DAL.DataModel.MemberClasssAttendanceView", b =>
                {
                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<int>("ClassOccurrenceID")
                        .HasColumnType("int");

                    b.Property<bool>("Baptised")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ClassID")
                        .HasColumnType("int");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ClassOccurrenceEndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ClassOccurrenceStartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(1)");

                    b.Property<string>("ImageReference")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsMainMember")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Mobile")
                        .HasColumnType("longtext");

                    b.Property<string>("Nickname")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<bool>("Present")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("ServantID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UNFileNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("UNFirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("UNLastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UNPersonalNumber")
                        .HasColumnType("longtext");

                    b.HasKey("MemberID", "ClassOccurrenceID");

                    b.ToTable((string)null);

                    b.ToView("MemberClasssAttendanceView", (string)null);
                });

            modelBuilder.Entity("SM.DAL.DataModel.MemberEventView", b =>
                {
                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.Property<int?>("AttendanceServantID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("AttendanceTimeStamp")
                        .HasColumnType("datetime(6)");

                    b.Property<bool?>("Attended")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Baptised")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Bus")
                        .HasColumnType("longtext");

                    b.Property<string>("CardStatus")
                        .HasColumnType("longtext");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(1)");

                    b.Property<string>("ImageReference")
                        .HasColumnType("longtext");

                    b.Property<bool?>("IsException")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsMainMember")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Mobile")
                        .HasColumnType("longtext");

                    b.Property<string>("Nickname")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<bool>("Registered")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("RegistrationNotes")
                        .HasColumnType("longtext");

                    b.Property<int?>("ServantID")
                        .HasColumnType("int");

                    b.Property<string>("Team")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UNFileNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("UNFirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("UNLastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UNPersonalNumber")
                        .HasColumnType("longtext");

                    b.HasKey("MemberID", "EventID");

                    b.ToTable((string)null);

                    b.ToView("MemberEventView", (string)null);
                });

            modelBuilder.Entity("SM.DAL.DataModel.Servant", b =>
                {
                    b.Property<int>("ServantID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ServantID"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Mobile1")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Mobile2")
                        .HasColumnType("longtext");

                    b.Property<string>("ServantName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ServantID");

                    b.ToTable("Servants");

                    b.HasData(
                        new
                        {
                            ServantID = -1,
                            IsActive = true,
                            Mobile1 = "",
                            ServantName = "admin",
                            UserID = "78769c53-5336-4411-8488-1983111d7be7"
                        });
                });

            modelBuilder.Entity("SM.DAL.DataModel.ServantClass", b =>
                {
                    b.Property<int>("ClassID")
                        .HasColumnType("int");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

                    b.HasKey("ClassID", "ServantID");

                    b.HasIndex("ServantID");

                    b.ToTable("ServantClasses");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Class", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Meeting", "Meeting")
                        .WithMany("Classes")
                        .HasForeignKey("MeetingID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Meeting");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassAttendance", b =>
                {
                    b.HasOne("SM.DAL.DataModel.ClassOccurrence", "ClassOccurrence")
                        .WithMany()
                        .HasForeignKey("ClassOccurrenceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Servant", "Servant")
                        .WithMany()
                        .HasForeignKey("ServantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClassOccurrence");

                    b.Navigation("Member");

                    b.Navigation("Servant");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassEvent", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Class", "Class")
                        .WithMany("ClassEvents")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Event", "Event")
                        .WithMany("ClassEvents")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Class");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassMember", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Class", "Class")
                        .WithMany("ClassMembers")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Member", "Member")
                        .WithMany("ClassMembers")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Class");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ClassOccurrence", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Class", "Class")
                        .WithMany("ClassOccurrences")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Class");
                });

            modelBuilder.Entity("SM.DAL.DataModel.EventRegistration", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Event", "Event")
                        .WithMany("EventRegistrations")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Member", "Member")
                        .WithMany("EventRegistrations")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Servant", "Servant")
                        .WithMany("EventRegistrations")
                        .HasForeignKey("ServantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Member");

                    b.Navigation("Servant");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Fund", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Aid", "Aid")
                        .WithMany()
                        .HasForeignKey("AidID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Member", "Member")
                        .WithMany("Funds")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Servant", "Servant")
                        .WithMany("Funds")
                        .HasForeignKey("ServantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aid");

                    b.Navigation("Member");

                    b.Navigation("Servant");
                });

            modelBuilder.Entity("SM.DAL.DataModel.MemberAid", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Aid", "Aid")
                        .WithMany("MemberAids")
                        .HasForeignKey("AidID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Member", "Member")
                        .WithMany("MemberAids")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Servant", "Servant")
                        .WithMany("MemberAids")
                        .HasForeignKey("ServantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aid");

                    b.Navigation("Member");

                    b.Navigation("Servant");
                });

            modelBuilder.Entity("SM.DAL.DataModel.ServantClass", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Class", "Class")
                        .WithMany("ServantClasses")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Servant", "Servant")
                        .WithMany("ServantClasses")
                        .HasForeignKey("ServantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Class");

                    b.Navigation("Servant");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Aid", b =>
                {
                    b.Navigation("MemberAids");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Class", b =>
                {
                    b.Navigation("ClassEvents");

                    b.Navigation("ClassMembers");

                    b.Navigation("ClassOccurrences");

                    b.Navigation("ServantClasses");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Event", b =>
                {
                    b.Navigation("ClassEvents");

                    b.Navigation("EventRegistrations");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Meeting", b =>
                {
                    b.Navigation("Classes");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Member", b =>
                {
                    b.Navigation("ClassMembers");

                    b.Navigation("EventRegistrations");

                    b.Navigation("Funds");

                    b.Navigation("MemberAids");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Servant", b =>
                {
                    b.Navigation("EventRegistrations");

                    b.Navigation("Funds");

                    b.Navigation("MemberAids");

                    b.Navigation("ServantClasses");
                });
#pragma warning restore 612, 618
        }
    }
}
