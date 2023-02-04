using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUploader.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "kettler");

            migrationBuilder.CreateTable(
                name: "Trainings",
                schema: "kettler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Device = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Calibration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Software = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Time = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecordIntervall = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Transmission = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Energy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrainingDateTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration_minutes = table.Column<double>(type: "float", nullable: false),
                    Power_Avg = table.Column<double>(type: "float", nullable: false),
                    RPM_Avg = table.Column<double>(type: "float", nullable: false),
                    Streak_days = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                schema: "kettler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pulse = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<int>(type: "int", nullable: false),
                    RPM = table.Column<int>(type: "int", nullable: false),
                    TimePassed_minutes = table.Column<double>(type: "float", nullable: false),
                    TimePassed_percent = table.Column<double>(type: "float", nullable: false),
                    Score_10sec = table.Column<double>(type: "float", nullable: false),
                    TrainingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalSchema: "kettler",
                        principalTable: "Trainings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_TrainingId",
                schema: "kettler",
                table: "Records",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_FileName",
                schema: "kettler",
                table: "Trainings",
                column: "FileName",
                unique: true,
                filter: "[FileName] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records",
                schema: "kettler");

            migrationBuilder.DropTable(
                name: "Trainings",
                schema: "kettler");
        }
    }
}
