using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Devices.Infrastructure.Persistence.Database.Migrations
{
    public partial class SpecifyIdTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByDevice",
                table: "Devices",
                type: "char(20)",
                unicode: false,
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedByDevice",
                table: "Devices",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(20)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 20);
        }
    }
}
