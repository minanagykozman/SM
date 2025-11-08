using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Medical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalAppoinments",
                columns: table => new
                {
                    MedicalAppointmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppoinmentDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    ServantID = table.Column<int>(type: "int", nullable: false),
                    PharmacistID = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Diagnosis = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAttended = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalAppoinments", x => x.MedicalAppointmentID);
                    table.ForeignKey(
                        name: "FK_MedicalAppoinments_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalAppoinments_Servants_PharmacistID",
                        column: x => x.PharmacistID,
                        principalTable: "Servants",
                        principalColumn: "ServantID");
                    table.ForeignKey(
                        name: "FK_MedicalAppoinments_Servants_ServantID",
                        column: x => x.ServantID,
                        principalTable: "Servants",
                        principalColumn: "ServantID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    MedicineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MedicineName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicalAppoinmentMedicalAppointmentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.MedicineID);
                    table.ForeignKey(
                        name: "FK_Medicines_MedicalAppoinments_MedicalAppoinmentMedicalAppoint~",
                        column: x => x.MedicalAppoinmentMedicalAppointmentID,
                        principalTable: "MedicalAppoinments",
                        principalColumn: "MedicalAppointmentID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAppoinments_MemberID",
                table: "MedicalAppoinments",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAppoinments_PharmacistID",
                table: "MedicalAppoinments",
                column: "PharmacistID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAppoinments_ServantID",
                table: "MedicalAppoinments",
                column: "ServantID");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_MedicalAppoinmentMedicalAppointmentID",
                table: "Medicines",
                column: "MedicalAppoinmentMedicalAppointmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "MedicalAppoinments");
        }
    }
}
