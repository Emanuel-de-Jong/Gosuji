using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KomiCN13",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "KomiCN19",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "KomiCN9",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "KomiJP13",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "KomiJP19",
                table: "SettingConfigs");

            migrationBuilder.DropColumn(
                name: "KomiJP9",
                table: "SettingConfigs");

            migrationBuilder.AlterColumn<int>(
                name: "SuggestionVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "SelfplayVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "PreVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "OpponentVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SuggestionVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SelfplayVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PreVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OpponentVisits",
                table: "TrainerSettingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KomiCN13",
                table: "SettingConfigs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KomiCN19",
                table: "SettingConfigs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KomiCN9",
                table: "SettingConfigs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KomiJP13",
                table: "SettingConfigs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KomiJP19",
                table: "SettingConfigs",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "KomiJP9",
                table: "SettingConfigs",
                type: "REAL",
                nullable: true);
        }
    }
}
