using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploader.Migrations
{
    public partial class NoAvg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Power_Avg",
                schema: "kettler",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "RPM_Avg",
                schema: "kettler",
                table: "Trainings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Power_Avg",
                schema: "kettler",
                table: "Trainings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RPM_Avg",
                schema: "kettler",
                table: "Trainings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
