using Gosuji.API.Data;
using Gosuji.Client.Data;
using Gosuji.Client.Models;
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
            GenerateUsers();
            GenerateSettingConfigs();
            GenerateUserRoles();
            GenerateUserStates();
            GenerateTrainerSettingConfigs();
            GeneratePresets();
            GenerateChangelogs();
            GenerateFeedbacks();
            GenerateDiscounts();
            GenerateSubscriptions();
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
                    Id = ELanguage.en.ToString(),
                    Name = "English",
                },
                new() {
                    Id = ELanguage.zh.ToString(),
                    Name = "汉语",
                },
                new() {
                    Id = ELanguage.ko.ToString(),
                    Name = "한국어",
                },
                new() {
                    Id = ELanguage.ja.ToString(),
                    Name = "日本語",
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
                PreMovesSwitch = false,
                PreMoves = 4,
                HideOptions = EHideOptions.RIGHT,
                ColorType = EMoveColor.RANDOM,
                WrongMoveCorrection = true,

                ForceOpponentCorners = EForceOpponentCorners.FIRST,
                CornerSwitch44 = true,
                CornerSwitch34 = true,
                CornerSwitch33 = false,
                CornerSwitch45 = false,
                CornerSwitch35 = false,
                CornerChance44 = 80,
                CornerChance34 = 100,
                CornerChance33 = 20,
                CornerChance45 = 20,
                CornerChance35 = 20,
                PreOptions = 2,
                PreOptionPercSwitch = false,
                PreOptionPerc = 20,

                SuggestionOptions = 6,
                HideWeakerOptions = true,
                MinVisitsPercSwitch = true,
                MinVisitsPerc = 5,
                MaxVisitDiffPercSwitch = false,
                MaxVisitDiffPerc = 40,

                OpponentOptions = 5,
                HideOpponentOptions = EHideOpponentOptions.ALWAYS,
                OpponentOptionPercSwitch = false,
                OpponentOptionPerc = 10,

                SelfplayPlaySpeed = 2.5,
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
            config.WrongMoveCorrection = false;
            config.HideOptions = EHideOptions.ALWAYS;
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

        private string CreateBackupCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public void GenerateUsers()
        {
            string password = "@Password1";
            userManager.CreateAsync(new()
            {
                UserName = "DefaultUser",
                Email = "defaultuser@gmail.com",
                EmailConfirmed = true,
                BackupCode = CreateBackupCode()
            }, password).Wait();
            userManager.CreateAsync(new()
            {
                UserName = "Bob",
                Email = "bob@gmail.com",
                EmailConfirmed = true,
                BackupCode = CreateBackupCode()
            }, password).Wait();
            userManager.CreateAsync(new()
            {
                UserName = "Jessy",
                Email = "jessy@gmail.com",
                EmailConfirmed = false,
                BackupCode = CreateBackupCode()
            }, password).Wait();
        }

        public void GenerateSettingConfigs()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            string[] userIds = dbContext.Users.Select(u => u.Id).ToArray();
            dbContext.SettingConfigs.AddRange([
                new() {
                    Id = userIds[0],
                    LanguageId = ELanguage.en.ToString(),
                    Theme = EThemeType.DARK,
                    MasterVolume = 100,
                    IsGetChangelogEmail = true,
                },
                new() {
                    Id = userIds[1],
                    LanguageId = ELanguage.en.ToString(),
                    Theme = EThemeType.DARK,
                    MasterVolume = 80,
                    IsGetChangelogEmail = false,
                },
                new() {
                    Id = userIds[2],
                    LanguageId = ELanguage.zh.ToString(),
                    Theme = EThemeType.LIGHT,
                    MasterVolume = 100,
                    IsGetChangelogEmail = true,
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateUserRoles()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            Dictionary<string, string> userIds = dbContext.Users.ToDictionary(u => u.UserName, u => u.Id);
            Dictionary<string, string> roleIds = dbContext.Roles.ToDictionary(r => r.Name, r => r.Id);
            dbContext.UserRoles.AddRange([
                new() {
                    UserId = userIds["DefaultUser"],
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
            config.PreMovesSwitch = random.Next(2) == 0;
            config.PreMoves = random.Next(5) + 1;
            config.HideOptions = (EHideOptions)random.Next(4);
            config.ColorType = (EMoveColor)random.Next(3) - 1;
            config.WrongMoveCorrection = random.Next(3) == 0;

            config.ForceOpponentCorners = (EForceOpponentCorners)random.Next(4);
            config.CornerSwitch33 = random.Next(5) == 0;
            config.CornerSwitch45 = random.Next(5) == 0;
            config.CornerSwitch35 = random.Next(5) == 0;
            config.PreOptionPercSwitch = random.Next(2) == 0;

            config.HideWeakerOptions = random.Next(3) == 0;
            config.MinVisitsPercSwitch = random.Next(3) == 0;
            config.MaxVisitDiffPercSwitch = random.Next(3) == 0;

            config.HideOpponentOptions = (EHideOpponentOptions)random.Next(3);
            config.OpponentOptionPercSwitch = random.Next(2) == 0;
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
                    FeedbackType = EFeedbackType.REPORT_BUG,
                    Subject = "I found a bug",
                    Message = "If you do this then everything explodes",
                    IsRead = false,
                    IsResolved = false,
                    CreateDate = date = date.AddDays(-1)
                },
                new() {
                    UserId = RandomItem(users).Id,
                    FeedbackType = EFeedbackType.SUPPORT,
                    Subject = "My subscription didn't activate",
                    Message = null,
                    IsRead = true,
                    IsResolved = false,
                    CreateDate = date = date.AddDays(-1)
                },
                new() {
                    UserId = RandomItem(users).Id,
                    FeedbackType = EFeedbackType.SUGGESTION,
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

        public void GenerateDiscounts()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            dbContext.Discounts.AddRange([
                new() {
                    Code = "LAUNCH",
                    Percent = 10,
                    ExpireDate = DateTimeOffset.UtcNow.AddMonths(1)
                },
            ]);
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        public void GenerateSubscriptions()
        {
            ApplicationDbContext dbContext = dbContextFactory.CreateDbContext();
            User[] users = dbContext.Users.ToArray();
            Discount[] discounts = dbContext.Discounts.ToArray();
            Subscription[] subscriptions = [
                new() {
                    UserId = users[0].Id,
                    SubscriptionType = ESubscriptionType.TIER_3,
                    Months = 3,
                    DiscountId = RandomItem(discounts).Id,
                    CreateDate = DateTimeOffset.UtcNow.AddMonths(-1),
                },
                new() {
                    UserId = users[0].Id,
                    SubscriptionType = ESubscriptionType.TIER_1,
                    Months = 3,
                    DiscountId = null,
                    CreateDate = DateTimeOffset.UtcNow.AddMonths(-2),
                },
                new() {
                    UserId = users[1].Id,
                    SubscriptionType = ESubscriptionType.TIER_1,
                    Months = 6,
                    DiscountId = null,
                    CreateDate = DateTimeOffset.UtcNow.AddMinutes(-1),
                },
            ];
            dbContext.Subscriptions.AddRange(subscriptions);
            dbContext.SaveChanges();

            Dictionary<string, User> usersById = users.ToDictionary(u => u.Id);
            foreach (Subscription subscription in subscriptions)
            {
                Subscription? currentSubscription = usersById[subscription.UserId].CurrentSubscription;
                if (currentSubscription == null ||
                    currentSubscription.EndDate < subscription.EndDate)
                {
                    usersById[subscription.UserId].CurrentSubscriptionId = subscription.Id;
                    currentSubscription = subscription;
                }
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
        }
    }
}
