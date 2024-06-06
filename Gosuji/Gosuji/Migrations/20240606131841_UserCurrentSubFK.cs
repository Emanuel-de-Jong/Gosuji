using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class UserCurrentSubFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserSubscriptions_CurrentSubscriptionId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CurrentSubscriptionId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentSubscriptionId1",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CurrentSubscriptionId",
                table: "AspNetUsers",
                column: "CurrentSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserSubscriptions_CurrentSubscriptionId",
                table: "AspNetUsers",
                column: "CurrentSubscriptionId",
                principalTable: "UserSubscriptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserSubscriptions_CurrentSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CurrentSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<long>(
                name: "CurrentSubscriptionId1",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CurrentSubscriptionId1",
                table: "AspNetUsers",
                column: "CurrentSubscriptionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserSubscriptions_CurrentSubscriptionId1",
                table: "AspNetUsers",
                column: "CurrentSubscriptionId1",
                principalTable: "UserSubscriptions",
                principalColumn: "Id");
        }
    }
}
