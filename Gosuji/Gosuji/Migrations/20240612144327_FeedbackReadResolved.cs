using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackReadResolved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Feedbacks");
        }
    }
}
