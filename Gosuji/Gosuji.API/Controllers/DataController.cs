using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.API.Services;
using Gosuji.Client.Data;
using Gosuji.Client.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    [EnableRateLimiting(RateLimitSetup.CONTROLLER_POLICY_NAME)]
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
        public async Task<ActionResult<Subscription?>> GetSubscription(bool includeDiscount = false)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Subscription? subscription = await dbContext.Users.Where(u => u.Id == GetUserId())
                .Select(u => u.CurrentSubscription)
                .FirstOrDefaultAsync();

            if (includeDiscount && subscription != null && subscription.DiscountId != null)
            {
                subscription.Discount = await dbContext.Discounts
                    .Where(d => d.Id == subscription.DiscountId)
                    .FirstOrDefaultAsync();
            }

            await dbContext.DisposeAsync();
            return Ok(subscription);
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
            if (end < start || end - start + 1 > 500)
            {
                return BadRequest("Invalid range. Max range 500.");
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            List<VMGame> games = await dbContext.Games
                .Where(g => g.UserId == GetUserId())
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

            if (game?.UserId != GetUserId())
            {
                return Forbid();
            }

            return Ok(game);
        }

        [HttpGet("{configId}")]
        public async Task<ActionResult<TrainerSettingConfig>> GetTrainerSettingConfig(long configId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            TrainerSettingConfig? config = await dbContext.TrainerSettingConfigs
                .Where(c => c.Id == configId)
                .FirstOrDefaultAsync();
            await dbContext.DisposeAsync();

            if (config == null)
            {
                return NotFound();
            }

            return Ok(config);
        }

        [HttpPost]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<long>> PostTrainerSettingConfig(TrainerSettingConfig config)
        {
            config.Id = 0;
            config.SetHash();

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
                sanitizeService.Sanitize(config);
                await dbContext.TrainerSettingConfigs.AddAsync(config);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.DisposeAsync();

            return Ok(config.Id);
        }

        [HttpPost]
        public async Task<ActionResult<long>> PostGameStat(GameStat gameStat)
        {
            gameStat.Id = 0;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            sanitizeService.Sanitize(gameStat);
            await dbContext.GameStats.AddAsync(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok(gameStat.Id);
        }

        [HttpPut]
        public async Task<ActionResult> PutGameStat(GameStat gameStat)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.GameStats.Any(gs => gs.Id == gameStat.Id) == false)
            {
                return NotFound();
            }

            sanitizeService.Sanitize(gameStat);
            dbContext.Update(gameStat);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpPost]
        [EnableRateLimiting("rl1")]
        public async Task<ActionResult<long>> PostGame(Game game)
        {
            game.Id = 0;
            game.UserId = GetUserId();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            sanitizeService.Sanitize(game);
            await dbContext.Games.AddAsync(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok(game.Id);
        }

        [HttpPut]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult> PutGame(Game game)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Game? oldGame = await dbContext.Games
                .Where(g => g.Id == game.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (oldGame == null)
            {
                return NotFound();
            }
            if (oldGame.UserId != GetUserId())
            {
                return Forbid();
            }

            if (game.UserId is null or "")
            {
                game.UserId = oldGame.UserId;
            }

            sanitizeService.Sanitize(game);
            dbContext.Update(game);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpPost]
        [EnableRateLimiting("rl1")]
        public async Task<ActionResult> PostFeedback(Feedback feedback)
        {
            feedback.Id = 0;
            feedback.UserId = GetUserId();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            sanitizeService.Sanitize(feedback);
            await dbContext.Feedbacks.AddAsync(feedback);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<SettingConfig>> GetSettingConfig()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            SettingConfig? settingConfig = await dbContext.SettingConfigs
                .Where(cs => cs.Id == GetUserId())
                .FirstOrDefaultAsync();
            await dbContext.DisposeAsync();
            return Ok(settingConfig);
        }

        [HttpPut]
        public async Task<ActionResult> PutSettingConfig(SettingConfig settingConfig)
        {
            settingConfig.Id = GetUserId();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.SettingConfigs.Any(cs => cs.Id == settingConfig.Id) == false)
            {
                return NotFound();
            }

            sanitizeService.Sanitize(settingConfig);
            dbContext.Update(settingConfig);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Dictionary<string, Language>>> GetLanguages()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Dictionary<string, Language> languages = await dbContext.Languages.ToDictionaryAsync(l => l.Id);
            await dbContext.DisposeAsync();
            return Ok(languages);
        }

        [HttpGet]
        public async Task<ActionResult<Dictionary<long, Preset>>> GetPresets()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Dictionary<long, Preset> presets = await dbContext.Presets
                .Where(p => p.UserId == null || p.UserId == GetUserId())
                .OrderBy(p => p.Id)
                .OrderBy(p => p.Order == null ? 1 : 0)
                .ThenBy(p => p.Order)
                .ToDictionaryAsync(p => p.Id);
            await dbContext.DisposeAsync();
            return Ok(presets);
        }

        [HttpPost]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult<long>> PostPreset(Preset preset)
        {
            preset.Id = 0;
            preset.UserId = GetUserId();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            sanitizeService.Sanitize(preset);
            await dbContext.Presets.AddAsync(preset);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok(preset.Id);
        }

        [HttpPut]
        [EnableRateLimiting("rl5")]
        public async Task<ActionResult> PutPreset(Preset preset)
        {
            if (preset.UserId == null)
            {
                return Forbid();
            }
            preset.UserId = GetUserId();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Preset? oldPreset = await dbContext.Presets
                .Where(p => p.Id == preset.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (oldPreset == null)
            {
                return NotFound();
            }
            if (oldPreset.UserId != GetUserId())
            {
                return Forbid();
            }

            sanitizeService.Sanitize(preset);
            dbContext.Update(preset);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpDelete("{presetId}")]
        public async Task<ActionResult> DeletePreset(long presetId)
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Preset? preset = await dbContext.Presets
                .Where(p => p.Id == presetId)
                .FirstOrDefaultAsync();
            if (preset == null)
            {
                return NotFound();
            }
            if (preset.UserId == null || preset.UserId != GetUserId())
            {
                return Forbid();
            }

            dbContext.Presets.Remove(preset);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<UserState>> GetUserState()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            UserState? userState = dbContext.UserStates
                .Where(us => us.Id == GetUserId())
                .FirstOrDefault();
            if (userState == null)
            {
                return NotFound();
            }
            await dbContext.DisposeAsync();
            return Ok(userState);
        }


        [HttpPut]
        public async Task<ActionResult> PutUserState(UserState userState)
        {
            if (userState.Id != GetUserId())
            {
                return Forbid();
            }

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Update(userState);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
            return Ok();
        }
    }
}
