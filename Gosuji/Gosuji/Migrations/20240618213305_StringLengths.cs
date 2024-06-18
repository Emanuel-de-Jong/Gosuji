using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class StringLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flag",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "Languages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }
    }
}
