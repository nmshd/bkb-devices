using Microsoft.EntityFrameworkCore.Migrations;

namespace Devices.Infrastructure.Persistence.Database.Migrations
{
    public partial class SaveClientIdWithIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Identities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Identities");
        }
    }
}
