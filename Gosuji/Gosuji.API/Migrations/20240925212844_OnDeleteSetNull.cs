using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class OnDeleteSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncodedGameDatas_Games_Id",
                table: "EncodedGameDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_EndgameStatId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_GameStatId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_MidgameStatId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_OpeningStatId",
                table: "Games");

            migrationBuilder.AddForeignKey(
                name: "FK_EncodedGameDatas_Games_Id",
                table: "EncodedGameDatas",
                column: "Id",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_EndgameStatId",
                table: "Games",
                column: "EndgameStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_GameStatId",
                table: "Games",
                column: "GameStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_MidgameStatId",
                table: "Games",
                column: "MidgameStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_OpeningStatId",
                table: "Games",
                column: "OpeningStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EncodedGameDatas_Games_Id",
                table: "EncodedGameDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_EndgameStatId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_GameStatId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_MidgameStatId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameStats_OpeningStatId",
                table: "Games");

            migrationBuilder.AddForeignKey(
                name: "FK_EncodedGameDatas_Games_Id",
                table: "EncodedGameDatas",
                column: "Id",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_EndgameStatId",
                table: "Games",
                column: "EndgameStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_GameStatId",
                table: "Games",
                column: "GameStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_MidgameStatId",
                table: "Games",
                column: "MidgameStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameStats_OpeningStatId",
                table: "Games",
                column: "OpeningStatId",
                principalTable: "GameStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
