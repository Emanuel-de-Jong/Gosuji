using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GosujiServer.Migrations
{
    public partial class UserMoveCountRenameVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Visits",
                table: "UserMoveCounts",
                newName: "KataGoVisits");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KataGoVisits",
                table: "UserMoveCounts",
                newName: "Visits");
        }
    }
}
