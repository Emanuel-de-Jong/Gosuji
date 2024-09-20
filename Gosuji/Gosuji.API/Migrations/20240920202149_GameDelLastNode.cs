using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class GameDelLastNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastNodeX",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LastNodeY",
                table: "Games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastNodeX",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastNodeY",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
