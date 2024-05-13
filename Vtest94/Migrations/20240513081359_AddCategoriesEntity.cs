using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vtest94.Migrations
{
    public partial class AddCategoriesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                column: "Name",
                values: new object[] { "Music" });
            migrationBuilder.InsertData(
                table: "Categories",
                column: "Name",
                values: new object[] { "Education" });
            migrationBuilder.InsertData(
                table: "Categories",
                column: "Name",
                values: new object[] { "Sports" });
            migrationBuilder.InsertData(
                table: "Categories",
                column: "Name",
                values: new object[] { "Technology" });
            migrationBuilder.InsertData(
                table: "Categories",
                column: "Name",
                values: new object[] { "Entertainment" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
