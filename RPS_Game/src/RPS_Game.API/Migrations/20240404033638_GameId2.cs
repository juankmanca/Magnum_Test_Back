using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPS_Game.API.Migrations
{
    /// <inheritdoc />
    public partial class GameId2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Games_GameId1",
                table: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_Rounds_GameId1",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "Rounds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "Rounds",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_GameId1",
                table: "Rounds",
                column: "GameId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Games_GameId1",
                table: "Rounds",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
