using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gosuji.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Changelogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Changelogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Percent = table.Column<float>(type: "REAL", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameStats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MoveNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Winrate = table.Column<int>(type: "INTEGER", nullable: true),
                    Score = table.Column<int>(type: "INTEGER", nullable: true),
                    Total = table.Column<int>(type: "INTEGER", nullable: false),
                    Right = table.Column<int>(type: "INTEGER", nullable: false),
                    RightStreak = table.Column<int>(type: "INTEGER", nullable: false),
                    RightTopStreak = table.Column<int>(type: "INTEGER", nullable: false),
                    Perfect = table.Column<int>(type: "INTEGER", nullable: false),
                    PerfectStreak = table.Column<int>(type: "INTEGER", nullable: false),
                    PerfectTopStreak = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KataGoVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    Config = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KataGoVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Short = table.Column<string>(type: "TEXT", nullable: false),
                    Flag = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainerSettingConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(type: "TEXT", nullable: false),
                    Boardsize = table.Column<int>(type: "INTEGER", nullable: false),
                    Handicap = table.Column<int>(type: "INTEGER", nullable: false),
                    ColorType = table.Column<int>(type: "INTEGER", nullable: false),
                    PreMovesSwitch = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreMoves = table.Column<int>(type: "INTEGER", nullable: false),
                    PreVisits = table.Column<int>(type: "INTEGER", nullable: false),
                    SelfplayVisits = table.Column<int>(type: "INTEGER", nullable: false),
                    SuggestionVisits = table.Column<int>(type: "INTEGER", nullable: false),
                    OpponentVisits = table.Column<int>(type: "INTEGER", nullable: false),
                    DisableAICorrection = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ruleset = table.Column<string>(type: "TEXT", nullable: true),
                    KomiChangeStyle = table.Column<string>(type: "TEXT", nullable: true),
                    Komi = table.Column<float>(type: "REAL", nullable: false),
                    PreOptions = table.Column<int>(type: "INTEGER", nullable: false),
                    PreOptionPerc = table.Column<float>(type: "REAL", nullable: false),
                    ForceOpponentCorners = table.Column<string>(type: "TEXT", nullable: true),
                    CornerSwitch44 = table.Column<bool>(type: "INTEGER", nullable: false),
                    CornerSwitch34 = table.Column<bool>(type: "INTEGER", nullable: false),
                    CornerSwitch33 = table.Column<bool>(type: "INTEGER", nullable: false),
                    CornerSwitch45 = table.Column<bool>(type: "INTEGER", nullable: false),
                    CornerSwitch35 = table.Column<bool>(type: "INTEGER", nullable: false),
                    CornerChance44 = table.Column<int>(type: "INTEGER", nullable: false),
                    CornerChance34 = table.Column<int>(type: "INTEGER", nullable: false),
                    CornerChance33 = table.Column<int>(type: "INTEGER", nullable: false),
                    CornerChance45 = table.Column<int>(type: "INTEGER", nullable: false),
                    CornerChance35 = table.Column<int>(type: "INTEGER", nullable: false),
                    SuggestionOptions = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowOptions = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowWeakerOptions = table.Column<bool>(type: "INTEGER", nullable: false),
                    MinVisitsPercSwitch = table.Column<bool>(type: "INTEGER", nullable: false),
                    MinVisitsPerc = table.Column<float>(type: "REAL", nullable: false),
                    MaxVisitDiffPercSwitch = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxVisitDiffPerc = table.Column<float>(type: "REAL", nullable: false),
                    OpponentOptionsSwitch = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpponentOptions = table.Column<int>(type: "INTEGER", nullable: false),
                    OpponentOptionPerc = table.Column<float>(type: "REAL", nullable: false),
                    ShowOpponentOptions = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerSettingConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LanguageId = table.Column<long>(type: "INTEGER", nullable: false),
                    IsDarkMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    Volume = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGetChangelogEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    KomiJP19 = table.Column<float>(type: "REAL", nullable: true),
                    KomiJP13 = table.Column<float>(type: "REAL", nullable: true),
                    KomiJP9 = table.Column<float>(type: "REAL", nullable: true),
                    KomiCN19 = table.Column<float>(type: "REAL", nullable: true),
                    KomiCN13 = table.Column<float>(type: "REAL", nullable: true),
                    KomiCN9 = table.Column<float>(type: "REAL", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingConfigs_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextValues",
                columns: table => new
                {
                    LanguageId = table.Column<long>(type: "INTEGER", nullable: false),
                    TextKeyId = table.Column<long>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextValues", x => new { x.LanguageId, x.TextKeyId });
                    table.ForeignKey(
                        name: "FK_TextValues_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextValues_TextKeys_TextKeyId",
                        column: x => x.TextKeyId,
                        principalTable: "TextKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SettingConfigId = table.Column<long>(type: "INTEGER", nullable: false),
                    CurrentSubscriptionId = table.Column<long>(type: "INTEGER", nullable: true),
                    IsBanned = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailConfirmedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_SettingConfigs_SettingConfigId",
                        column: x => x.SettingConfigId,
                        principalTable: "SettingConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    FeedbackTypeId = table.Column<long>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_FeedbackTypes_FeedbackTypeId",
                        column: x => x.FeedbackTypeId,
                        principalTable: "FeedbackTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    TrainerSettingConfigId = table.Column<long>(type: "INTEGER", nullable: false),
                    KataGoVersionId = table.Column<long>(type: "INTEGER", nullable: true),
                    GameStatId = table.Column<long>(type: "INTEGER", nullable: true),
                    OpeningStatId = table.Column<long>(type: "INTEGER", nullable: true),
                    MidgameStatId = table.Column<long>(type: "INTEGER", nullable: true),
                    EndgameStatId = table.Column<long>(type: "INTEGER", nullable: true),
                    Thumbnail = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Result = table.Column<int>(type: "INTEGER", nullable: true),
                    PrevNodeX = table.Column<int>(type: "INTEGER", nullable: false),
                    PrevNodeY = table.Column<int>(type: "INTEGER", nullable: false),
                    Boardsize = table.Column<int>(type: "INTEGER", nullable: false),
                    Handicap = table.Column<int>(type: "INTEGER", nullable: false),
                    Color = table.Column<int>(type: "INTEGER", nullable: false),
                    Ruleset = table.Column<string>(type: "TEXT", nullable: true),
                    Komi = table.Column<float>(type: "REAL", nullable: false),
                    SGF = table.Column<string>(type: "TEXT", nullable: true),
                    Ratios = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Suggestions = table.Column<byte[]>(type: "BLOB", nullable: true),
                    MoveTypes = table.Column<byte[]>(type: "BLOB", nullable: true),
                    ChosenNotPlayedCoords = table.Column<byte[]>(type: "BLOB", nullable: true),
                    IsFinished = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsThirdPartySGF = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_GameStats_EndgameStatId",
                        column: x => x.EndgameStatId,
                        principalTable: "GameStats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_GameStats_GameStatId",
                        column: x => x.GameStatId,
                        principalTable: "GameStats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_GameStats_MidgameStatId",
                        column: x => x.MidgameStatId,
                        principalTable: "GameStats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_GameStats_OpeningStatId",
                        column: x => x.OpeningStatId,
                        principalTable: "GameStats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_KataGoVersions_KataGoVersionId",
                        column: x => x.KataGoVersionId,
                        principalTable: "KataGoVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Games_TrainerSettingConfigs_TrainerSettingConfigId",
                        column: x => x.TrainerSettingConfigId,
                        principalTable: "TrainerSettingConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    TrainerSettingConfigId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presets_TrainerSettingConfigs_TrainerSettingConfigId",
                        column: x => x.TrainerSettingConfigId,
                        principalTable: "TrainerSettingConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Ip = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMoveCounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Moves = table.Column<int>(type: "INTEGER", nullable: false),
                    KataGoVisits = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMoveCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMoveCounts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionTypeId = table.Column<long>(type: "INTEGER", nullable: false),
                    DiscountId = table.Column<long>(type: "INTEGER", nullable: true),
                    Months = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifyDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_SubscriptionTypes_SubscriptionTypeId",
                        column: x => x.SubscriptionTypeId,
                        principalTable: "SubscriptionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SettingConfigId",
                table: "AspNetUsers",
                column: "SettingConfigId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_FeedbackTypeId",
                table: "Feedbacks",
                column: "FeedbackTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserId",
                table: "Feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_EndgameStatId",
                table: "Games",
                column: "EndgameStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameStatId",
                table: "Games",
                column: "GameStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_KataGoVersionId",
                table: "Games",
                column: "KataGoVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_MidgameStatId",
                table: "Games",
                column: "MidgameStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_OpeningStatId",
                table: "Games",
                column: "OpeningStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_TrainerSettingConfigId",
                table: "Games",
                column: "TrainerSettingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserId",
                table: "Games",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Presets_TrainerSettingConfigId",
                table: "Presets",
                column: "TrainerSettingConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Presets_UserId",
                table: "Presets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingConfigs_LanguageId",
                table: "SettingConfigs",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TextValues_TextKeyId",
                table: "TextValues",
                column: "TextKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId",
                table: "UserActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMoveCounts_UserId",
                table: "UserMoveCounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_DiscountId",
                table: "UserSubscriptions",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_SubscriptionTypeId",
                table: "UserSubscriptions",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Changelogs");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Presets");

            migrationBuilder.DropTable(
                name: "TextValues");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "UserMoveCounts");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "FeedbackTypes");

            migrationBuilder.DropTable(
                name: "GameStats");

            migrationBuilder.DropTable(
                name: "KataGoVersions");

            migrationBuilder.DropTable(
                name: "TrainerSettingConfigs");

            migrationBuilder.DropTable(
                name: "TextKeys");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "SubscriptionTypes");

            migrationBuilder.DropTable(
                name: "SettingConfigs");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
