using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class SettingConfigAudio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "SettingConfigs",
                newName: "MasterVolume");

            migrationBuilder.AddColumn<bool>(
                name: "IsPreMoveStoneSound",
                table: "SettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelfplayStoneSound",
                table: "SettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StoneVolume",
                table: "SettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreMoveStoneSound",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "IsSelfplayStoneSound",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "StoneVolume",
                table: "SettingConfigs");

            migrationBuilder.RenameColumn(
                name: "MasterVolume",
                table: "SettingConfigs",
                newName: "Volume");
        }
    }
}
