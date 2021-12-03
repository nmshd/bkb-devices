using Microsoft.EntityFrameworkCore.Migrations;

namespace Devices.Infrastructure.Persistence.Database.Migrations
{
    public partial class RemoveIdentityTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Identities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Identities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
