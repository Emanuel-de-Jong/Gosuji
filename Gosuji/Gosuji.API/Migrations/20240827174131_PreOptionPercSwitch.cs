using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class PreOptionPercSwitch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpponentOptionsSwitch",
                table: "TrainerSettingConfigs",
                newName: "OpponentOptionPercSwitch");

            migrationBuilder.AddColumn<bool>(
                name: "PreOptionPercSwitch",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreOptionPercSwitch",
                table: "TrainerSettingConfigs");

            migrationBuilder.RenameColumn(
                name: "OpponentOptionPercSwitch",
                table: "TrainerSettingConfigs",
                newName: "OpponentOptionsSwitch");
        }
    }
}
