using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class EncodedGameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncodedData",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "EncodedGameDatas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncodedGameDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EncodedGameDatas_Games_Id",
                        column: x => x.Id,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncodedGameDatas");

            migrationBuilder.AddColumn<byte[]>(
                name: "EncodedData",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
