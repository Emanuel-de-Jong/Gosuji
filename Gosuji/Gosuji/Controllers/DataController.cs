using Ganss.Xss;
using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Gosuji.Client.ViewModels;
using Gosuji.Data;
using Gosuji.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DataController : IDataService
    {
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;
        private SanitizeService sanitizeService;

        public DataController(IDbContextFactory<ApplicationDbContext> _dbContextFactory, SanitizeService _sanitizeService)
        {
            dbContextFactory = _dbContextFactory;
            sanitizeService = _sanitizeService;
        }

        [HttpGet]
        public async Task<Changelog[]> GetChangelogs()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Changelog[] changelogs = await dbContext.Changelogs.ToArrayAsync();
            await dbContext.DisposeAsync();
            return changelogs;
        }

        [HttpGet("{userId}/{start}/{end}")]
        public async Task<VMGame[]?> GetUserGames(string userId,
            [Range(1, 100_000)] int start = 1,
            [Range(1, 100_000)] int end = 500)
        {
            if (end < start || end - start + 1 > 500)
            {
                return null;
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            VMGame[] games = await dbContext.Games
                .Where(g => g.UserId == userId)
                .Where(g => g.IsDeleted == false)
                .Skip(start - 1)
                .Take(end - start + 1)
                .Include(g => g.GameStat)
                .Include(g => g.OpeningStat)
                .Include(g => g.MidgameStat)
                .Include(g => g.EndgameStat)
                .Select(g => new VMGame
                {
                    Id = g.Id,
                    GameStat = g.GameStat,
                    OpeningStat = g.OpeningStat,
                    MidgameStat = g.MidgameStat,
                    EndgameStat = g.EndgameStat,
                    Name = g.Name,
                    Result = g.Result,
                    Boardsize = g.Boardsize,
                    Handicap = g.Handicap,
                    Color = g.Color,
                    IsFinished = g.IsFinished,
                    IsThirdPartySGF = g.IsThirdPartySGF,
                    CreateDate = g.CreateDate,
                    ModifyDate = g.ModifyDate
                })
                .ToArrayAsync();
            await dbContext.DisposeAsync();

            if (games.Length == 0)
            {
                return null;
            }

            return games;
        }

        [HttpGet("{gameId}")]
        public async Task<Game?> GetGame(long gameId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Game? game = await dbContext.Games.Where(g => g.Id == gameId).FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return game;
        }

        [HttpPost]
        public async Task<long> PostTrainerSettingConfig(TrainerSettingConfig config)
        {
            sanitizeService.Sanitize(config);

            if (config.Hash == null || config.Hash == "")
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

        [HttpPost]
        public async Task<long> PostGameStat(GameStat gameStat)
        {
            sanitizeService.Sanitize(gameStat);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.GameStats.AddAsync(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return gameStat.Id;
        }

        [HttpPut]
        public async Task PutGameStat(GameStat gameStat)
        {
            sanitizeService.Sanitize(gameStat);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.GameStats.Update(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        [HttpPost]
        public async Task<long> PostGame(Game game)
        {
            sanitizeService.Sanitize(game);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return game.Id;
        }

        [HttpPut]
        public async Task PutGame(Game game)
        {
            sanitizeService.Sanitize(game);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Games.Update(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        [HttpPost]
        public async Task PostFeedback(Feedback feedback)
        {
            sanitizeService.Sanitize(feedback);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.Feedbacks.AddAsync(feedback);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        [HttpGet("{userId}")]
        public async Task<SettingConfig> GetSettingConfig(string userId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            SettingConfig settingConfig = await dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.SettingConfig)
                .FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return settingConfig;
        }

        [HttpPut]
        public async Task PutSettingConfig(SettingConfig settingConfig)
        {
            sanitizeService.Sanitize(settingConfig);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.SettingConfigs.Update(settingConfig);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        [HttpGet]
        public async Task<Dictionary<string, Language>> GetLanguages()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Dictionary<string, Language> languages = await dbContext.Languages.ToDictionaryAsync(l => l.Short);
            await dbContext.DisposeAsync();
            return languages;
        }
    }
}
