using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class TrainerSettingConfigVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OpponentVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelfplayVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuggestionVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpponentVisits",
                table: "TrainerSettingConfigs");

            migrationBuilder.DropColumn(
                name: "PreVisits",
                table: "TrainerSettingConfigs");

            migrationBuilder.DropColumn(
                name: "SelfplayVisits",
                table: "TrainerSettingConfigs");

            migrationBuilder.DropColumn(
                name: "SuggestionVisits",
                table: "TrainerSettingConfigs");
        }
    }
}
