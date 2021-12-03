using Microsoft.EntityFrameworkCore.Migrations;

namespace Devices.Infrastructure.Persistence.Database.Migrations
{
    public partial class AddCreatedByDeviceToDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByDevice",
                table: "Devices",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByDevice",
                table: "Devices");
        }
    }
}
