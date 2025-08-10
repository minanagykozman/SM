using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedRokesPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //clear old data
            migrationBuilder.Sql("DELETE FROM RolePermissions;");
            migrationBuilder.Sql("DELETE FROM UserRoles;");
            migrationBuilder.Sql("DELETE FROM Roles;");
            migrationBuilder.Sql("DELETE FROM Permissions;");


            // 1. Seeding Permissions
            migrationBuilder.InsertData(table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Events.Manage" },
                    { 2, "Events.View" },
                    { 3, "Events.Register" },
                    { 4, "Events.Attendance" },
                    { 5, "Class.View" },
                    { 6, "Class.Manage" },
                    { 7, "Class.Attendance" },
                    { 8, "Class.ManageVisitations" },
                    { 9, "Class.RegisterVisitation" },
                    { 10, "Aids.Manage" },
                    { 11, "Aids.View" },
                    { 12, "Aids.Register" },
                    { 13, "Funds.Manage" },
                    { 14, "Funds.View" },
                    { 15, "Funds.Review" },
                    { 16, "Servants.Manage" },
                    { 17, "Members.Manage" },
                    { 18, "Members.Delete" },
                    { 19, "Members.View" },
                    { 20, "Members.ManageCards" }
                });

            // 2. Seeding Roles
            migrationBuilder.InsertData(table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", "SuperAdmin", "SUPERADMIN" },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", "Servant", "SERVANT" },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", "ChurchAdmin", "CHURCHADMIN" },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", "ClassAdmin", "CLASSADMIN" },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", "Priest", "PRIEST" },
                    { "b13e42cd-43a2-44fa-ade8-9f8b36f34fbd", "FundAdmin", "FUNDADMIN" },
                    { "f07a2c3e-7e6b-4ea1-89f5-0abfb8f2749f", "FundReviewer", "FUNDREVIEWER" },
                    { "cfe2b1d4-96fa-4db0-98ab-a74f53d0619e", "FundViewer", "FUNDVIEWER" }
                });

            // 3. Seeding Role-Permission relationships
            migrationBuilder.InsertData(table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 1 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 1 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 1 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 2 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 2 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 2 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 2 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 2 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 3 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 3 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 3 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 3 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 4 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 4 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 4 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 4 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 5 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 5 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 5 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 5 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 5 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 6 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 6 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 7 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 7 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 7 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 7 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 8 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 8 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 8 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 9 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 9 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 9 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 9 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 10 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 10 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 10 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 11 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 11 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 11 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 11 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 11 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 12 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 12 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 12 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 12 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 13 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 13 },
                    { "b13e42cd-43a2-44fa-ade8-9f8b36f34fbd", 13 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 14 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 14 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 14 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 14 },
                    { "cfe2b1d4-96fa-4db0-98ab-a74f53d0619e", 14 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 15 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 15 },
                    { "f07a2c3e-7e6b-4ea1-89f5-0abfb8f2749f", 15 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 16 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 16 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 17 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 17 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 17 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 17 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 18 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 18 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 18 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 19 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 19 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 19 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 19 },
                    { "8e1d672d-d03e-4f4b-9084-67c53ad03e3e", 19 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 20 },
                    { "e4f5a9c0-3a44-4b9e-9a6c-22a73f9bbce0", 20 },
                    { "9a6f2f13-4970-4f6c-a3ff-7309c01910e5", 20 },
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", 20 }
                });

            //add default account
            migrationBuilder.InsertData(table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "b11f7196-85c8-425c-8f9b-11311229de22", "383d20f8-ac3b-46ff-b322-4f21cda036dc" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
