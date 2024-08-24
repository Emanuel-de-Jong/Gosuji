using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameShowToHideWeakerOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShowWeakerOptions",
                table: "TrainerSettingConfigs",
                newName: "HideWeakerOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HideWeakerOptions",
                table: "TrainerSettingConfigs",
                newName: "ShowWeakerOptions");
        }
    }
}
