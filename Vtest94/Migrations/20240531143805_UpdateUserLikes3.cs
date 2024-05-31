using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vtest94.Migrations
{
    public partial class UpdateUserLikes3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrowserFingerprint",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "UserLikes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrowserFingerprint",
                table: "UserLikes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "UserLikes",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");
        }
    }
}
