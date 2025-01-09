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
    [Migration("20250109103151_Ver0.1")]
    partial class Ver01
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

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

                    b.Property<DateTime>("ClassOccurrenceDate")
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

            modelBuilder.Entity("SM.DAL.DataModel.EventAttendance", b =>
                {
                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("EventID", "MemberID");

                    b.HasIndex("MemberID");

                    b.HasIndex("ServantID");

                    b.ToTable("EventAttendances");
                });

            modelBuilder.Entity("SM.DAL.DataModel.EventRegistration", b =>
                {
                    b.Property<int>("EventID")
                        .HasColumnType("int");

                    b.Property<int>("MemberID")
                        .HasColumnType("int");

                    b.Property<bool>("IsException")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int>("ServantID")
                        .HasColumnType("int");

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

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(1)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsMainMember")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Mobile")
                        .HasColumnType("longtext");

                    b.Property<string>("Nickname")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<string>("School")
                        .HasColumnType("longtext");

                    b.Property<string>("UNFileNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("UNFirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("UNLastName")
                        .HasColumnType("longtext");

                    b.Property<string>("UNPersonalNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("Work")
                        .HasColumnType("longtext");

                    b.HasKey("MemberID");

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

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ServantName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ServantID");

                    b.ToTable("Servants");
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

            modelBuilder.Entity("SM.DAL.DataModel.EventAttendance", b =>
                {
                    b.HasOne("SM.DAL.DataModel.Event", "Event")
                        .WithMany("EventAttendances")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Member", "Member")
                        .WithMany("EventAttendances")
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SM.DAL.DataModel.Servant", "Servant")
                        .WithMany("EventAttendances")
                        .HasForeignKey("ServantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Member");

                    b.Navigation("Servant");
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

                    b.Navigation("EventAttendances");

                    b.Navigation("EventRegistrations");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Meeting", b =>
                {
                    b.Navigation("Classes");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Member", b =>
                {
                    b.Navigation("ClassMembers");

                    b.Navigation("EventAttendances");

                    b.Navigation("EventRegistrations");

                    b.Navigation("Funds");

                    b.Navigation("MemberAids");
                });

            modelBuilder.Entity("SM.DAL.DataModel.Servant", b =>
                {
                    b.Navigation("EventAttendances");

                    b.Navigation("EventRegistrations");

                    b.Navigation("Funds");

                    b.Navigation("MemberAids");

                    b.Navigation("ServantClasses");
                });
#pragma warning restore 612, 618
        }
    }
}
