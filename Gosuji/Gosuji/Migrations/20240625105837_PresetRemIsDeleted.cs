using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class PresetRemIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presets_AspNetUsers_UserId",
                table: "Presets");

            migrationBuilder.DropIndex(
                name: "IX_Presets_UserId",
                table: "Presets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Presets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Presets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Presets_UserId",
                table: "Presets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presets_AspNetUsers_UserId",
                table: "Presets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
