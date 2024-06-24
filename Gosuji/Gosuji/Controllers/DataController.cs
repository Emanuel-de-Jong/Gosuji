using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Client.ViewModels;
using Gosuji.Data;
using Gosuji.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    [EnableRateLimiting(G.ControllerRateLimitPolicyName)]
    public class DataController : CustomControllerBase
    {
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;
        private SanitizeService sanitizeService;

        public DataController(IDbContextFactory<ApplicationDbContext> _dbContextFactory, SanitizeService _sanitizeService)
        {
            dbContextFactory = _dbContextFactory;
            sanitizeService = _sanitizeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Changelog[]>> GetChangelogs()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Changelog[] changelogs = await dbContext.Changelogs.ToArrayAsync();
            await dbContext.DisposeAsync();
            return Ok(changelogs);
        }

        [HttpGet("{start}/{end}")]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<List<VMGame>>> GetUserGames([Range(1, 100_000)] int start = 1,
            [Range(1, 100_000)] int end = 500)
        {
            string userId = GetUserId();

            if (end < start || end - start + 1 > 500)
            {
                return BadRequest("Invalid range. Max range 500.");
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            List<VMGame> games = await dbContext.Games
                .Where(g => g.UserId == userId)
                .Where(g => g.IsDeleted == false)
                .OrderByDescending(g => g.Id)
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
                .ToListAsync();
            await dbContext.DisposeAsync();

            return Ok(games);
        }

        [HttpGet("{gameId}")]
        public async Task<ActionResult<Game>> GetGame(long gameId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Game? game = await dbContext.Games.Where(g => g.Id == gameId).FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return Ok(game);
        }

        [HttpPost]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<long>> PostTrainerSettingConfig(TrainerSettingConfig config)
        {
            sanitizeService.Sanitize(config);

            if (config.Hash is null or "")
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

            return Ok(config.Id);
        }

        [HttpPost]
        public async Task<ActionResult<long>> PostGameStat(GameStat gameStat)
        {
            sanitizeService.Sanitize(gameStat);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.GameStats.AddAsync(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok(gameStat.Id);
        }

        [HttpPut]
        public async Task<ActionResult> PutGameStat(GameStat gameStat)
        {
            sanitizeService.Sanitize(gameStat);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.GameStats.Any(gs => gs.Id == gameStat.Id) == false)
            {
                return BadRequest("GameStat not found.");
            }

            dbContext.GameStats.Update(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpPost]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<long>> PostGame(Game game)
        {
            game.UserId = GetUserId();

            sanitizeService.Sanitize(game);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok(game.Id);
        }

        [HttpPut]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult> PutGame(Game game)
        {
            game.UserId = GetUserId();

            sanitizeService.Sanitize(game);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.Games.Any(g => g.Id == game.Id) == false)
            {
                return BadRequest("Game not found.");
            }

            dbContext.Games.Update(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpPost]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult> PostFeedback(Feedback feedback)
        {
            feedback.UserId = GetUserId();

            sanitizeService.Sanitize(feedback);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            await dbContext.Feedbacks.AddAsync(feedback);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<SettingConfig>> GetSettingConfig()
        {
            string userId = GetUserId();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            SettingConfig settingConfig = await dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.SettingConfig)
                .FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return Ok(settingConfig);
        }

        [HttpPut]
        public async Task<ActionResult> PutSettingConfig(SettingConfig settingConfig)
        {
            sanitizeService.Sanitize(settingConfig);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.SettingConfigs.Any(cs => cs.Id == settingConfig.Id) == false)
            {
                return BadRequest("SettingConfig not found.");
            }

            dbContext.SettingConfigs.Update(settingConfig);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Dictionary<string, Language>>> GetLanguages()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Dictionary<string, Language> languages = await dbContext.Languages.ToDictionaryAsync(l => l.Short);
            await dbContext.DisposeAsync();
            return Ok(languages);
        }
    }
}
