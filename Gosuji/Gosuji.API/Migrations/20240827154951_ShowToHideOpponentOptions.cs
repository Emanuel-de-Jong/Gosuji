using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class ShowToHideOpponentOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowOpponentOptions",
                table: "TrainerSettingConfigs",
                newName: "HideOpponentOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HideOpponentOptions",
                table: "TrainerSettingConfigs",
                newName: "ShowOpponentOptions");
        }
    }
}
