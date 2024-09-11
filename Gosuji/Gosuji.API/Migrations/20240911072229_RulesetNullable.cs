using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class RulesetNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Boardsize",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ChosenNotPlayedCoords",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Handicap",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Komi",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "MoveTypes",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PlayerResults",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Ruleset",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SGF",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "GameStats");

            migrationBuilder.DropColumn(
                name: "Winrate",
                table: "GameStats");

            migrationBuilder.RenameColumn(
                name: "Suggestions",
                table: "Games",
                newName: "EncodedData");

            migrationBuilder.RenameColumn(
                name: "PrevNodeY",
                table: "Games",
                newName: "ShouldIgnoreStats");

            migrationBuilder.RenameColumn(
                name: "PrevNodeX",
                table: "Games",
                newName: "LastNodeY");

            migrationBuilder.RenameColumn(
                name: "IsFinished",
                table: "Games",
                newName: "LastNodeX");

            migrationBuilder.RenameColumn(
                name: "MoveNumber",
                table: "GameStats",
                newName: "To");

            migrationBuilder.AlterColumn<long>(
                name: "OpeningStatId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "GameStatId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductVersion",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "From",
                table: "GameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductVersion",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "From",
                table: "GameStats");

            migrationBuilder.RenameColumn(
                name: "ShouldIgnoreStats",
                table: "Games",
                newName: "PrevNodeY");

            migrationBuilder.RenameColumn(
                name: "LastNodeY",
                table: "Games",
                newName: "PrevNodeX");

            migrationBuilder.RenameColumn(
                name: "LastNodeX",
                table: "Games",
                newName: "IsFinished");

            migrationBuilder.RenameColumn(
                name: "EncodedData",
                table: "Games",
                newName: "Suggestions");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "GameStats",
                newName: "MoveNumber");

            migrationBuilder.AlterColumn<long>(
                name: "OpeningStatId",
                table: "Games",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<long>(
                name: "GameStatId",
                table: "Games",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "Boardsize",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "ChosenNotPlayedCoords",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Handicap",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Komi",
                table: "Games",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<byte[]>(
                name: "MoveTypes",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PlayerResults",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Ruleset",
                table: "Games",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SGF",
                table: "Games",
                type: "TEXT",
                maxLength: 100000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "GameStats",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Winrate",
                table: "GameStats",
                type: "INTEGER",
                nullable: true);
        }
    }
}
