using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlankNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSubjectToInspection = table.Column<bool>(type: "bit", nullable: false),
                    InspectionPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextInspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsInternational = table.Column<bool>(type: "bit", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspections");
        }
    }
}
