using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class GameStatStreakToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerfectStreak",
                table: "GameStats");

            migrationBuilder.DropColumn(
                name: "PerfectTopStreak",
                table: "GameStats");

            migrationBuilder.DropColumn(
                name: "RightStreak",
                table: "GameStats");

            migrationBuilder.DropColumn(
                name: "RightTopStreak",
                table: "GameStats");

            migrationBuilder.AddColumn<int>(
                name: "PerfectStreak",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerfectTopStreak",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RightStreak",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RightTopStreak",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerfectStreak",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "PerfectTopStreak",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RightStreak",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "RightTopStreak",
                table: "Games");

            migrationBuilder.AddColumn<int>(
                name: "PerfectStreak",
                table: "GameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerfectTopStreak",
                table: "GameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RightStreak",
                table: "GameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RightTopStreak",
                table: "GameStats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
