using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModifyMedicineAppointmentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_MedicalAppoinments_MedicalAppoinmentMedicalAppoint~",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_MedicalAppoinmentMedicalAppointmentID",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "MedicalAppoinmentMedicalAppointmentID",
                table: "Medicines");

            migrationBuilder.CreateTable(
                name: "MedicalAppointmentMedicines",
                columns: table => new
                {
                    MedicalAppointmentID = table.Column<int>(type: "int", nullable: false),
                    MedicineID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalAppointmentMedicines", x => new { x.MedicalAppointmentID, x.MedicineID });
                    table.ForeignKey(
                        name: "FK_MedicalAppointmentMedicines_MedicalAppoinments_MedicalAppoin~",
                        column: x => x.MedicalAppointmentID,
                        principalTable: "MedicalAppoinments",
                        principalColumn: "MedicalAppointmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalAppointmentMedicines_Medicines_MedicineID",
                        column: x => x.MedicineID,
                        principalTable: "Medicines",
                        principalColumn: "MedicineID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalAppointmentMedicines_MedicineID",
                table: "MedicalAppointmentMedicines",
                column: "MedicineID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalAppointmentMedicines");

            migrationBuilder.AddColumn<int>(
                name: "MedicalAppoinmentMedicalAppointmentID",
                table: "Medicines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_MedicalAppoinmentMedicalAppointmentID",
                table: "Medicines",
                column: "MedicalAppoinmentMedicalAppointmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_MedicalAppoinments_MedicalAppoinmentMedicalAppoint~",
                table: "Medicines",
                column: "MedicalAppoinmentMedicalAppointmentID",
                principalTable: "MedicalAppoinments",
                principalColumn: "MedicalAppointmentID");
        }
    }
}
