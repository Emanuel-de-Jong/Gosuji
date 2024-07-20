using Gosuji.Client.Data;
using Gosuji.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Helpers
{
    public class TestDataGen
    {
        private const int PRESETS = 10;

        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }
        private UserManager<User> userManager { get; set; }
        private RoleManager<IdentityRole> roleManager { get; set; }

        public TestDataGen(IServiceProvider serviceProvider)
        {
            dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        }

        public void ClearDb()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            dbContext.Dispose();
        }

        private T RandomItem<T>(T[] array)
        {
            Random random = new();
            int randomIndex = random.Next(array.Length);
            return array[randomIndex];
        }

        public void GenerateTestData()
        {
            GenerateEssentials();
            GenerateSettingConfigs();
            GenerateUsers();
            GenerateUserRoles();
            GenerateUserStates();
            GenerateTrainerSettingConfigs();
            GeneratePresets();
            GenerateChangelogs();
            GenerateFeedbacks();
        }

        public void GenerateEssentials()
        {
            GenerateRoles();
            GenerateLanguages();
            GenerateGeneralTrainerSettingConfigs();
            GenerateGeneralPresets();
        }

        public void GenerateRoles()
        {
            roleManager.CreateAsync(new("Owner")).Wait();
        }

        public void GenerateLanguages()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            dbContext.Languages.AddRange([
                new() {
                    Name = "English",
                    Short = "en",
                },
                new() {
                    Name = "汉语",
                    Short = "zh",
                },
                new() {
                    Name = "한국어",
                    Short = "ko",
                },
                new() {
                    Name = "日本語",
                    Short = "ja",
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        private TrainerSettingConfig GetTrainerSettingConfigBase()
        {
            return new TrainerSettingConfig
            {
                Boardsize = 19,
                Handicap = 0,
                ColorType = 0,
                PreMovesSwitch = false,
                PreMoves = 5,
                DisableAICorrection = false,

                Ruleset = "Japanese",
                KomiChangeStyle = "Automatic",
                Komi = 6.5,

                PreOptions = 2,
                PreOptionPerc = 20,
                ForceOpponentCorners = "Both",
                CornerSwitch44 = true,
                CornerSwitch34 = true,
                CornerSwitch33 = false,
                CornerSwitch45 = false,
                CornerSwitch35 = false,
                CornerChance44 = 8,
                CornerChance34 = 12,
                CornerChance33 = 2,
                CornerChance45 = 2,
                CornerChance35 = 2,

                SuggestionOptions = 6,
                ShowOptions = true,
                ShowWeakerOptions = false,
                MinVisitsPercSwitch = true,
                MinVisitsPerc = 10,
                MaxVisitDiffPercSwitch = false,
                MaxVisitDiffPerc = 40,

                OpponentOptionsSwitch = false,
                OpponentOptions = 5,
                OpponentOptionPerc = 10,
                ShowOpponentOptions = false,
            };
        }

        public void GenerateGeneralTrainerSettingConfigs()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            TrainerSettingConfig config;
            config = GetTrainerSettingConfigBase();
            dbContext.TrainerSettingConfigs.Add(config.SetHash());

            config = GetTrainerSettingConfigBase();
            config.PreMovesSwitch = true;
            dbContext.TrainerSettingConfigs.Add(config.SetHash());

            config = GetTrainerSettingConfigBase();
            config.DisableAICorrection = true;
            config.ShowOptions = false;
            dbContext.TrainerSettingConfigs.Add(config.SetHash());
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateGeneralPresets()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            int id = 1;
            dbContext.Presets.AddRange([
                new() {
                    Name = "Default",
                    Order = id,
                    TrainerSettingConfigId = id++,
                },
                new() {
                    Name = "Quick start",
                    Order = id,
                    TrainerSettingConfigId = id++,
                },
                new() {
                    Name = "Just play",
                    Order = id,
                    TrainerSettingConfigId = id++,
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateSettingConfigs()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            Dictionary<string, long> languageIds = dbContext.Languages.ToDictionary(l => l.Short, l => l.Id);
            dbContext.SettingConfigs.AddRange([
                new() {
                    LanguageId = languageIds["en"],
                    IsDarkMode = true,
                    MasterVolume = 100,
                    IsGetChangelogEmail = true,
                },
                new() {
                    LanguageId = languageIds["en"],
                    IsDarkMode = true,
                    MasterVolume = 80,
                    IsGetChangelogEmail = false,
                },
                new() {
                    LanguageId = languageIds["zh"],
                    IsDarkMode = false,
                    MasterVolume = 100,
                    IsGetChangelogEmail = true,
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateUsers()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            long[] settingConfigIds = dbContext.SettingConfigs.Select(sc => sc.Id).ToArray();
            dbContext.Dispose();

            int settingConfigIndex = 0;
            string password = "@Password1";
            userManager.CreateAsync(new()
            {
                UserName = "Admino",
                Email = "admino@gmail.com",
                EmailConfirmed = true,
                SettingConfigId = settingConfigIds[settingConfigIndex++],
            }, password).Wait();
            userManager.CreateAsync(new()
            {
                UserName = "Bob",
                Email = "bob@gmail.com",
                EmailConfirmed = true,
                SettingConfigId = settingConfigIds[settingConfigIndex++],
            }, password).Wait();
            userManager.CreateAsync(new()
            {
                UserName = "Jessy",
                Email = "jessy@gmail.com",
                EmailConfirmed = false,
                SettingConfigId = settingConfigIds[settingConfigIndex++],
            }, password).Wait();
        }

        public void GenerateUserRoles()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            Dictionary<string, string> userIds = dbContext.Users.ToDictionary(u => u.UserName, u => u.Id);
            Dictionary<string, string> roleIds = dbContext.Roles.ToDictionary(r => r.Name, r => r.Id);
            dbContext.UserRoles.AddRange([
                new() {
                    UserId = userIds["Admino"],
                    RoleId = roleIds["Owner"],
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateUserStates()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            string[] userIds = dbContext.Users.Select(u => u.Id).ToArray();
            foreach (string id in userIds)
            {
                dbContext.UserStates.Add(new()
                {
                    Id = id,
                    LastPresetId = 1,
                });
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public TrainerSettingConfig GetRandomTrainerSettingConfig()
        {
            Random random = new();
            TrainerSettingConfig config = GetTrainerSettingConfigBase();
            config.Handicap = random.Next(10);
            config.ColorType = random.Next(3) - 1;
            config.PreMovesSwitch = random.Next(2) == 0;
            config.PreMoves = random.Next(11);
            config.DisableAICorrection = random.Next(2) == 0;
            return config.SetHash();
        }

        public void GenerateTrainerSettingConfigs()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            for (int i = 0; i < PRESETS; i++)
            {
                dbContext.TrainerSettingConfigs.Add(GetRandomTrainerSettingConfig());
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GeneratePresets()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            User[] users = dbContext.Users.ToArray();
            long[] trainerSettingConfigIds = dbContext.TrainerSettingConfigs
                .OrderByDescending(c => c.Id)
                .Select(c => c.Id)
                .ToArray();
            for (int i = 0; i < PRESETS; i++)
            {
                User user = users[i % users.Length];
                dbContext.Presets.Add(new()
                {
                    Name = $"{user.UserName} {Guid.NewGuid().ToString()[..5]}",
                    UserId = user.Id,
                    TrainerSettingConfigId = trainerSettingConfigIds[i],
                });
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateChangelogs()
        {
            DateTimeOffset date = DateTimeOffset.UtcNow.AddMonths(-1);
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            dbContext.Changelogs.AddRange([
                new() {
                    Version = null,
                    Name = "GoMagic",
                    Description = "It's a great place to learn Go!",
                    Date = date = date.AddDays(-1)
                },
                new() {
                    Version = "1.1",
                    Name = "Bug fixes",
                    Description = "- The thing works now.\n- That is fixed.",
                    Date = date = date.AddDays(-1)
                },
                new() {
                    Version = "1.0",
                    Name = "Initial Release",
                    Description = "Initial release of the application.",
                    Date = date = date.AddDays(-1)
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateFeedbacks()
        {
            DateTimeOffset date = DateTimeOffset.UtcNow.AddMonths(-1);
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            User[] users = dbContext.Users.ToArray();
            dbContext.Feedbacks.AddRange([
                new() {
                    UserId = RandomItem(users).Id,
                    FeedbackType = EFeedbackType.ReportBug,
                    Subject = "I found a bug",
                    Message = "If you do this then everything explodes",
                    IsRead = false,
                    IsResolved = false,
                    CreateDate = date = date.AddDays(-1)
                },
                new() {
                    UserId = RandomItem(users).Id,
                    FeedbackType = EFeedbackType.Support,
                    Subject = "My subscription didn't activate",
                    Message = null,
                    IsRead = true,
                    IsResolved = false,
                    CreateDate = date = date.AddDays(-1)
                },
                new() {
                    UserId = RandomItem(users).Id,
                    FeedbackType = EFeedbackType.Suggestion,
                    Subject = "This would be cool",
                    Message = "When you\nAnd then\n\nCould you do that?",
                    IsRead = true,
                    IsResolved = true,
                    CreateDate = date = date.AddDays(-1)
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }
    }
}
