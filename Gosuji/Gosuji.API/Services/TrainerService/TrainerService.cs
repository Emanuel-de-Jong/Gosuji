using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Gosuji.API.Services.TrainerService
{
    public class TrainerService : IAsyncDisposable
    {
        private KataGoPool pool;
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;
        public string UserId { get; set; }

        public Game Game { get; set; }
        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public NullableTrainerSettings? NullableTrainerSettings { get; set; }
        public KataGo? KataGo { get; set; }
        public MoveTree MoveTree { get; set; } = new();

        private bool isAnalyzing = false;

        public TrainerService(string userId, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            UserId = userId;
            pool = kataGoPool;
            this.dbContextFactory = dbContextFactory;

            Game = new();
        }

        private async Task StartKataGo()
        {
            await (await GetKataGo()).Restart();
            (await GetKataGo()).SetBoardsize(TrainerSettingConfig.Boardsize);
            (await GetKataGo()).SetRuleset(NullableTrainerSettings.Ruleset);
            (await GetKataGo()).SetHandicap(TrainerSettingConfig.Handicap);
            (await GetKataGo()).SetKomi(NullableTrainerSettings.Komi);
        }

        public async Task Return()
        {
            await pool.Return(UserId);
        }

        public async Task<bool> UserHasInstance()
        {
            return pool.UserHasInstance(UserId);
        }

        public async Task Init(TrainerSettingConfig trainerSettingConfig, NullableTrainerSettings nullableTrainerSettings,
            bool isThirdPartySGF)
        {
            TrainerSettingConfig = trainerSettingConfig;
            NullableTrainerSettings = nullableTrainerSettings;
            Game.IsThirdPartySGF = isThirdPartySGF;

            await StartKataGo();
        }

        public async Task SyncBoard(Move[] moves)
        {
            (await GetKataGo()).ClearBoard();
            (await GetKataGo()).SetHandicap(TrainerSettingConfig.Handicap);
            (await GetKataGo()).PlayRange(moves);

            MoveTree.CurrentNode = MoveTree.RootNode;
            foreach (Move move in moves)
            {
                MoveTree.Add(move);
            }
        }

        public async Task SetRuleset(string ruleset)
        {
            (await GetKataGo()).SetRuleset(ruleset);
        }

        public async Task SetKomi(double komi)
        {
            (await GetKataGo()).SetKomi(komi);
        }

        public async Task<MoveSuggestion> AnalyzeMove(Move move)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            MoveSuggestion suggestion = (await GetKataGo()).AnalyzeMove(move);
            MoveTree.CurrentNode.Suggestions.AnalyzeMoveSuggestion = suggestion;

            isAnalyzing = false;
            return suggestion;
        }

        public async Task<MoveSuggestionList> Analyze(EMoveColor color, int maxVisits, double minVisitsPerc, double maxVisitDiffPerc)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            MoveSuggestionList suggestions = (await GetKataGo()).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc);
            MoveTree.CurrentNode.Suggestions = suggestions;

            isAnalyzing = false;
            return suggestions;
        }

        public async Task Play(Move move)
        {
            MoveTree.Add(move);
            (await GetKataGo()).Play(move);
        }

        private async Task<KataGo> GetKataGo()
        {
            if (KataGo == null || KataGo.IsPaused)
            {
                KataGo = await pool.Get(UserId);
                await StartKataGo();
                // TODO: Play till the current move
            }

            return KataGo;
        }

        public async ValueTask DisposeAsync()
        {
            //await Save();
        }
    }
}
