using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class TypesToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_FeedbackTypes_FeedbackTypeId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_KataGoVersions_KataGoVersionId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_SubscriptionTypes_SubscriptionTypeId",
                table: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "FeedbackTypes");

            migrationBuilder.DropTable(
                name: "SubscriptionTypes");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_SubscriptionTypeId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_FeedbackTypeId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "UserSubscriptions");

            migrationBuilder.RenameColumn(
                name: "SubscriptionTypeId",
                table: "UserSubscriptions",
                newName: "SubscriptionType");

            migrationBuilder.RenameColumn(
                name: "FeedbackTypeId",
                table: "Feedbacks",
                newName: "FeedbackType");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Suggestions",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SGF",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ruleset",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Ratios",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "MoveTypes",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "KataGoVersionId",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ChosenNotPlayedCoords",
                table: "Games",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_KataGoVersions_KataGoVersionId",
                table: "Games",
                column: "KataGoVersionId",
                principalTable: "KataGoVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_KataGoVersions_KataGoVersionId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "SubscriptionType",
                table: "UserSubscriptions",
                newName: "SubscriptionTypeId");

            migrationBuilder.RenameColumn(
                name: "FeedbackType",
                table: "Feedbacks",
                newName: "FeedbackTypeId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "UserSubscriptions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Suggestions",
                table: "Games",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<string>(
                name: "SGF",
                table: "Games",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Ruleset",
                table: "Games",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Ratios",
                table: "Games",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Games",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<byte[]>(
                name: "MoveTypes",
                table: "Games",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AlterColumn<long>(
                name: "KataGoVersionId",
                table: "Games",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<byte[]>(
                name: "ChosenNotPlayedCoords",
                table: "Games",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.CreateTable(
                name: "FeedbackTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_SubscriptionTypeId",
                table: "UserSubscriptions",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FeedbackTypeId",
                table: "Feedbacks",
                column: "FeedbackTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_UserId",
                table: "Feedbacks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_FeedbackTypes_FeedbackTypeId",
                table: "Feedbacks",
                column: "FeedbackTypeId",
                principalTable: "FeedbackTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_KataGoVersions_KataGoVersionId",
                table: "Games",
                column: "KataGoVersionId",
                principalTable: "KataGoVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_SubscriptionTypes_SubscriptionTypeId",
                table: "UserSubscriptions",
                column: "SubscriptionTypeId",
                principalTable: "SubscriptionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
