using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionUserIdNotUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_AspNetUsers_UserId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_AspNetUsers_UserId",
                table: "UserSubscriptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
