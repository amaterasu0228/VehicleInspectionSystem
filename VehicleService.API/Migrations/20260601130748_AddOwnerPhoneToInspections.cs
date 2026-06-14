using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VehicleService.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerPhoneToInspections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerPhone",
                table: "Inspections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerPhone",
                table: "Inspections");
        }
    }
}
