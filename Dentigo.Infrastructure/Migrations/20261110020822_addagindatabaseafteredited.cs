using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dentigo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addagindatabaseafteredited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "treatmentPlanStatuscs",
                table: "TreatmentPlans");

            migrationBuilder.AddColumn<string>(
                name: "treatmentPlanStatus",
                table: "TreatmentPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "treatmentPlanStatus",
                table: "TreatmentPlans");

            migrationBuilder.AddColumn<string>(
                name: "treatmentPlanStatuscs",
                table: "TreatmentPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Open");
        }
    }
}
