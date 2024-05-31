using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vtest94.Migrations
{
    public partial class AdjustVideoTableNewEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoStats_Videos_VideoId",
                table: "VideoStats");

            migrationBuilder.DropIndex(
                name: "IX_VideoStats_VideoId",
                table: "VideoStats");

            migrationBuilder.AddColumn<int>(
                name: "VideoStatsId",
                table: "Videos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoStatsId",
                table: "Videos",
                column: "VideoStatsId",
                unique: true,
                filter: "[VideoStatsId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_VideoStats_VideoStatsId",
                table: "Videos",
                column: "VideoStatsId",
                principalTable: "VideoStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_VideoStats_VideoStatsId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoStatsId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoStatsId",
                table: "Videos");

            migrationBuilder.CreateIndex(
                name: "IX_VideoStats_VideoId",
                table: "VideoStats",
                column: "VideoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoStats_Videos_VideoId",
                table: "VideoStats",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
