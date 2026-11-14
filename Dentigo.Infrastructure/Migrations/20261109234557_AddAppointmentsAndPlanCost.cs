using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dentigo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentsAndPlanCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgreedCost",
                table: "TreatmentPlans",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "treatmentPlanStatuscs",
                table: "TreatmentPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Open");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentPlanId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TreatmentPlanId",
                table: "Payments",
                column: "TreatmentPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_TreatmentPlans_TreatmentPlanId",
                table: "Payments",
                column: "TreatmentPlanId",
                principalTable: "TreatmentPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_TreatmentPlans_TreatmentPlanId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TreatmentPlanId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AgreedCost",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "treatmentPlanStatuscs",
                table: "TreatmentPlans");

            migrationBuilder.DropColumn(
                name: "TreatmentPlanId",
                table: "Payments");
        }
    }
}
