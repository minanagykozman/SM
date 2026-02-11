using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MemberBatismData_VisitationPremissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VisitationNotes",
                table: "Members",
                newName: "BaptismChurch");

            migrationBuilder.AddColumn<DateOnly>(
                name: "BaptismDate",
                table: "Members",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitationDate",
                table: "Members",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.InsertData(table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 27, "Visitation.Manage" },
                    { 28, "Visitation.View" }
                });

            migrationBuilder.InsertData(table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 27 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 28 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 27 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 28 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 27 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 28 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 27 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 28 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 27 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaptismDate",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastVisitationDate",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "BaptismChurch",
                table: "Members",
                newName: "VisitationNotes");
        }
    }
}
