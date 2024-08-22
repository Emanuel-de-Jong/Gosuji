using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class UserDelSettingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SettingConfigs_SettingConfigId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SettingConfigId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SettingConfigId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SettingConfigId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SettingConfigId",
                table: "AspNetUsers",
                column: "SettingConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SettingConfigs_SettingConfigId",
                table: "AspNetUsers",
                column: "SettingConfigId",
                principalTable: "SettingConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
