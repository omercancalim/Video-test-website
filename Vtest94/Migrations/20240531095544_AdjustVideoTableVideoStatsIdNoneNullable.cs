using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vtest94.Migrations
{
    public partial class AdjustVideoTableVideoStatsIdNoneNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoStatsId",
                table: "Videos");

            migrationBuilder.AlterColumn<int>(
                name: "VideoStatsId",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoStatsId",
                table: "Videos",
                column: "VideoStatsId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoStatsId",
                table: "Videos");

            migrationBuilder.AlterColumn<int>(
                name: "VideoStatsId",
                table: "Videos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoStatsId",
                table: "Videos",
                column: "VideoStatsId",
                unique: true,
                filter: "[VideoStatsId] IS NOT NULL");
        }
    }
}
