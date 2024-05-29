using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vtest94.Migrations
{
    public partial class AddArbitraryUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArbitraryUsername",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArbitraryUsername",
                table: "AspNetUsers");
        }
    }
}
