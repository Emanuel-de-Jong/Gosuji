using Gosuji.API.Data;
using Gosuji.API.Services;
using Gosuji.API.Services.TrainerService;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Controllers
{
    [Authorize]
    public class TrainerHub : CustomHubBase
    {
        private static ConcurrentDictionary<string, TrainerService> trainerServices = new();
        private static IDbContextFactory<ApplicationDbContext>? dbContextFactory;

        private SanitizeService sanitizeService;
        private KataGoPool pool;

        public TrainerHub(SanitizeService sanitizeService, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            this.sanitizeService = sanitizeService;
            pool = kataGoPool;

            TrainerHub.dbContextFactory ??= dbContextFactory;
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            TrainerService service = new(GetUserId(), pool, dbContextFactory);
            await service.SetSubscription();

            trainerServices.TryAdd(connectionId, service);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            if (trainerServices.TryRemove(connectionId, out TrainerService service))
            {
                await service.DisposeAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<HubResponse> UserHasInstance()
        {
            return OkData(await trainerServices[Context.ConnectionId].UserHasInstance());
        }

        public async Task<HubResponse> Init(TrainerSettingConfig trainerSettingConfig, bool isThirdPartySGF)
        {
            await trainerServices[Context.ConnectionId].Init(trainerSettingConfig, isThirdPartySGF);
            return Ok;
        }

        public async Task<HubResponse> UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            await trainerServices[Context.ConnectionId].UpdateTrainerSettingConfig(trainerSettingConfig);
            return Ok;
        }

        public async Task<HubResponse> SyncBoard(Move[] moves)
        {
            await trainerServices[Context.ConnectionId].SyncBoard(moves);
            return Ok;
        }

        public async Task<HubResponse> AnalyzeMove(Move move)
        {
            return OkData(await trainerServices[Context.ConnectionId].AnalyzeMove(move));
        }

        public async Task<HubResponse> Analyze(EMoveColor color,
            [Required, Range(2, 100_000)] int maxVisits,
            [Required, Range(0, 100)] double minVisitsPerc,
            [Required, Range(0, 100)] double maxVisitDiffPerc)
        {
            return OkData(await trainerServices[Context.ConnectionId].Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc));
        }

        public async Task<HubResponse> Play(Move move)
        {
            await trainerServices[Context.ConnectionId].Play(move);
            return Ok;
        }
    }
}
