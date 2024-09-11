using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.API.Migrations
{
    /// <inheritdoc />
    public partial class TrainerSettingNullables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KomiChangeStyle",
                table: "TrainerSettingConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "Ruleset",
                table: "TrainerSettingConfigs",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<double>(
                name: "Komi",
                table: "TrainerSettingConfigs",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ruleset",
                table: "TrainerSettingConfigs",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Komi",
                table: "TrainerSettingConfigs",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KomiChangeStyle",
                table: "TrainerSettingConfigs",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
