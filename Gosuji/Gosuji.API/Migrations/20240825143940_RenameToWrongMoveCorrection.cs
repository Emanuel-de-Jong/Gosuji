using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameToWrongMoveCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisableAICorrection",
                table: "TrainerSettingConfigs",
                newName: "WrongMoveCorrection");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WrongMoveCorrection",
                table: "TrainerSettingConfigs",
                newName: "DisableAICorrection");
        }
    }
}
