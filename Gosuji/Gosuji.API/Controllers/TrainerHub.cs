﻿using Gosuji.API.Data;
using Gosuji.API.Services;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

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

        public async Task<HubResponse> LoadGame(string gameId)
        {
            return OkData(await trainerServices[Context.ConnectionId].LoadGame(gameId));
        }

        public async Task<HubResponse> Init(TrainerSettingConfig trainerSettingConfig,
            TreeNode<Move>? thirdPartyMoves, string? name)
        {
            return OkData(await trainerServices[Context.ConnectionId].Init(trainerSettingConfig,
                thirdPartyMoves, name));
        }

        public async Task<HubResponse> UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            await trainerServices[Context.ConnectionId].UpdateTrainerSettingConfig(trainerSettingConfig);
            return Ok;
        }

        public async Task<HubResponse> Analyze(EMoveOrigin moveOrigin, EMoveColor color, bool isMainBranch, Move[]? moves)
        {
            return OkData(await trainerServices[Context.ConnectionId].Analyze(moveOrigin, color, isMainBranch, moves));
        }

        public async Task<HubResponse> AnalyzeMove(Move move)
        {
            return OkData(await trainerServices[Context.ConnectionId].AnalyzeMove(move));
        }

        public async Task<HubResponse> PlayPlayer(Move move, EPlayerResult playerResult, Coord? chosenNotPlayedCoord,
            int rightStreak, int perfectStreak, int? rightTopStreak, int? perfectTopStreak)
        {
            await trainerServices[Context.ConnectionId].PlayPlayer(move, playerResult, chosenNotPlayedCoord,
                rightStreak, perfectStreak, rightTopStreak, perfectTopStreak);
            return Ok;
        }

        public async Task<HubResponse> PlayForcedCorner(Move move)
        {
            return OkData(await trainerServices[Context.ConnectionId].PlayForcedCorner(move));
        }
    }
}
