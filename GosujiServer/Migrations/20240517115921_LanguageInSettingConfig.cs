using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GosujiServer.Migrations
{
    public partial class LanguageInSettingConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LanguageId",
                table: "SettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SettingConfigs_LanguageId",
                table: "SettingConfigs",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingConfigs_Languages_LanguageId",
                table: "SettingConfigs",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SettingConfigs_Languages_LanguageId",
                table: "SettingConfigs");

            migrationBuilder.DropIndex(
                name: "IX_SettingConfigs_LanguageId",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "SettingConfigs");
        }
    }
}
