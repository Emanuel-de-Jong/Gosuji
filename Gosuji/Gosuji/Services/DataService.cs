using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Gosuji.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Gosuji.Services
{
    public class DataService : IServerService, IDataService
    {
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;

        public DataService(IDbContextFactory<ApplicationDbContext> _dbContextFactory)
        {
            dbContextFactory = _dbContextFactory;
        }

        public static void CreateEndpoints(WebApplication app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/DataService");
            group.MapGet("/GetChangelogs", (IDataService service) => service.GetChangelogs());
            group.MapGet("/GetUserLanguageIds", (IDataService service) => service.GetUserLanguageIds());
            group.MapGet("/GetUserGames/{userId}", (string userId, IDataService service) => service.GetUserGames(userId));
            group.MapGet("/GetGame/{gameId}", (long gameId, IDataService service) => service.GetGame(gameId));
            group.MapPost("/PostTrainerSettingConfig", (TrainerSettingConfig config, IDataService service) => service.PostTrainerSettingConfig(config));
            group.MapPost("/PostGameStat", (GameStat gamestat, IDataService service) => service.PostGameStat(gamestat));
            group.MapPut("/PutGameStat", (GameStat gamestat, IDataService service) => service.PutGameStat(gamestat));
            group.MapPost("/PostGame", (Game game, IDataService service) => service.PostGame(game));
            group.MapPut("/PutGame", (Game game, IDataService service) => service.PutGame(game));
            group.MapPost("/PostFeedback", (Feedback feedback, IDataService service) => service.PostFeedback(feedback));
            group.MapGet("/GetSettingConfig/{userId}", (string userId, IDataService service) => service.GetSettingConfig(userId));
            group.MapPut("/PutSettingConfig", (SettingConfig settingConfig, IDataService service) => service.PutSettingConfig(settingConfig));
            group.MapGet("/GetLanguages", (IDataService service) => service.GetLanguages());
        }

        public async Task<Changelog[]> GetChangelogs()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Changelog[] changelogs = await dbContext.Changelogs.ToArrayAsync();
            await dbContext.DisposeAsync();
            return changelogs;
        }

        public async Task<Dictionary<string, long>> GetUserLanguageIds()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            Dictionary<string, long> userLanguageIds = [];
            foreach (User user in dbContext.Users.Include(u => u.SettingConfig))
            {
                userLanguageIds[user.Id] = user.SettingConfig.LanguageId;
            }

            await dbContext.DisposeAsync();

            return userLanguageIds;
        }

        public async Task<Game[]> GetUserGames(string userId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Game[] games = await dbContext.Games
                .Where(g => g.UserId == userId)
                .Where(g => g.IsDeleted == false)
                .Include(g => g.GameStat)
                .Include(g => g.OpeningStat)
                .Include(g => g.MidgameStat)
                .Include(g => g.EndgameStat)
                .ToArrayAsync();
            await dbContext.DisposeAsync();
            return games;
        }

        public async Task<Game?> GetGame(long gameId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Game? game = await dbContext.Games
                .Where(g => g.Id == gameId)
                .Include(g => g.Ratios)
                .Include(g => g.Suggestions)
                .Include(g => g.MoveTypes)
                .Include(g => g.ChosenNotPlayedCoords)
                .FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return game;
        }

        public async Task<long> PostTrainerSettingConfig(TrainerSettingConfig config)
        {
            if (config.Hash.IsNullOrEmpty())
            {
                config.SetHash();
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            TrainerSettingConfig? existingConfig = await dbContext.TrainerSettingConfigs
                .Where(c => c.Hash == config.Hash)
                .FirstOrDefaultAsync();

            if (existingConfig != null)
            {
                config.Id = existingConfig.Id;
            }
            else
            {
                await dbContext.TrainerSettingConfigs.AddAsync(config);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.DisposeAsync();

            return config.Id;
        }

        public async Task<long> PostGameStat(GameStat gameStat)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.GameStats.AddAsync(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return gameStat.Id;
        }

        public async Task PutGameStat(GameStat gameStat)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.GameStats.Update(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        public async Task<long> PostGame(Game game)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return game.Id;
        }

        public async Task PutGame(Game game)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Games.Update(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        public async Task PostFeedback(Feedback feedback)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.Feedbacks.AddAsync(feedback);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        public async Task<SettingConfig> GetSettingConfig(string userId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            SettingConfig settingConfig = await dbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.SettingConfig.Language)
                .Select(u => u.SettingConfig)
                .FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return settingConfig;
        }

        public async Task PutSettingConfig(SettingConfig settingConfig)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.SettingConfigs.Update(settingConfig);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        public async Task<Dictionary<string, Language>> GetLanguages()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Dictionary<string, Language> languages = await dbContext.Languages.ToDictionaryAsync(l => l.Short);
            await dbContext.DisposeAsync();
            return languages;
        }
    }
}
