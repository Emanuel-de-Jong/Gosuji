using Gosuji.API.Data;
using Gosuji.API.Helpers;
using Gosuji.Client.Data;
using Gosuji.Client.Models;
using Gosuji.Client.Models.Trainer;
using Gosuji.Client.Services.Trainer;
using Microsoft.EntityFrameworkCore;

namespace Gosuji.API.Services.TrainerService
{
    public class TrainerService : IAsyncDisposable
    {
        public const int MIDGAME_MOVE_NUMBER = 41;
        public const int ENDGAME_MOVE_NUMBER = 121;

        public string UserId { get; set; }
        private KataGoPool pool;
        private IDbContextFactory<ApplicationDbContext> dbContextFactory;

        public Subscription? Subscription { get; set; }

        public TrainerSettingConfig? TrainerSettingConfig { get; set; }
        public Game? Game { get; set; }
        public MoveTree? MoveTree { get; set; }
        public KataGo? KataGo { get; set; }

        private bool isFirstInit = true;
        private Random rnd = new();
        private bool isAnalyzing = false;
        private bool shouldBeImperfectSuggestion = false;
        private string? name = null;
        private bool isExistingGame = false;

        public TrainerService(string userId, KataGoPool kataGoPool, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            this.UserId = userId;
            this.pool = kataGoPool;
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<bool> Init(TrainerSettingConfig trainerSettingConfig,
            TreeNode<Move>? thirdPartyMoves, string? name, string? gameId)
        {
            if (isFirstInit)
            {
                isFirstInit = false;

                if (pool.UserHasInstance(UserId))
                {
                    return false;
                }

                // So there is less time where the user could start 2 instances
                await pool.Get(UserId);

                ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
                Subscription = await dbContext.Subscriptions.FirstOrDefaultAsync(s => s.UserId == UserId);
                await dbContext.DisposeAsync();
            }
            else
            {
                await Save();
            }

            TrainerSettingConfig = trainerSettingConfig;
            TrainerSettingConfig.SubscriptionType = Subscription?.SubscriptionType;

            Game = new();
            MoveTree = new();

            if (gameId != null)
            {
                ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
                Game = await dbContext.Games
                    .Where(g => g.Id == gameId)
                    .Include(g => g.EncodedGameData)
                    .Include(g => g.GameStat)
                    .Include(g => g.OpeningStat)
                    .Include(g => g.MidgameStat)
                    .Include(g => g.EndgameStat)
                    .FirstOrDefaultAsync();
                await dbContext.DisposeAsync();

                GameDecoder gameDecoder = new();
                MoveTree = gameDecoder.Decode(Game.EncodedGameData.Data);
            }

            Game.IsThirdPartySGF = thirdPartyMoves != null;
            this.name = name ?? Game.Name;
            isExistingGame = gameId != null;

            await StartKataGo();

            if (thirdPartyMoves != null)
            {
                // Skip the root as MoveTree already has one
                foreach (TreeNode<Move> childTreeNode in thirdPartyMoves.Children)
                {
                    ApplyThirdPartyMoves(childTreeNode);
                }
            }

            return true;
        }

        public async Task UpdateTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            TrainerSettingConfig = trainerSettingConfig;
            TrainerSettingConfig.SubscriptionType = Subscription?.SubscriptionType;
        }

        public async Task<AnalyzeResponse> Analyze(EMoveOrigin moveOrigin, EMoveColor color, bool isMainBranch, Move[]? moves)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            if (moves != null)
            {
                await SyncBoard(moves);
            }

            int maxVisits = 0;
            double minVisitsPerc = 0;
            double maxVisitDiffPerc = 100;
            int moveOptions = 1;
            switch (moveOrigin)
            {
                case EMoveOrigin.PLAYER:
                    maxVisits = TrainerSettingConfig.GetSuggestionVisits;
                    minVisitsPerc = TrainerSettingConfig.MinVisitsPercSwitch ? TrainerSettingConfig.MinVisitsPerc : minVisitsPerc;
                    maxVisitDiffPerc = TrainerSettingConfig.MaxVisitDiffPercSwitch ? TrainerSettingConfig.MaxVisitDiffPerc : maxVisitDiffPerc;
                    moveOptions = TrainerSettingConfig.SuggestionOptions;
                    break;
                case EMoveOrigin.OPPONENT:
                    maxVisits = TrainerSettingConfig.GetOpponentVisits;
                    minVisitsPerc = 10;
                    maxVisitDiffPerc = 50;
                    moveOptions = TrainerSettingConfig.OpponentOptions;
                    break;
                case EMoveOrigin.PRE:
                    maxVisits = TrainerSettingConfig.GetPreVisits;
                    minVisitsPerc = 10;
                    maxVisitDiffPerc = 50;
                    moveOptions = TrainerSettingConfig.PreOptions;
                    break;
                case EMoveOrigin.SELFPLAY:
                    maxVisits = TrainerSettingConfig.GetSelfplayVisits;
                    break;
            }

            MoveSuggestionList suggestions = (await GetKataGo()).Analyze(color, maxVisits, minVisitsPerc, maxVisitDiffPerc, moveOptions);
            MoveTree.CurrentNode.Suggestions = suggestions;

            int? playIndex = null;

            double? result = GetResult(suggestions);
            MoveTree.CurrentNode.Result = result;

            if (result == null)
            {
                playIndex = CalcPlayIndex(suggestions, moveOrigin);

                if (playIndex != null)
                {
                    Move move = new(color, suggestions.Suggestions[playIndex.Value].Coord);
                    await Play(move, moveOrigin);
                }
            }
            else
            {
                MoveTree.Add(Move.PASS_MOVE);
                MoveTree.CurrentNode.MoveOrigin = EMoveOrigin.PASS;
            }

            MoveTree.MainBranch = isMainBranch ? MoveTree.CurrentNode : MoveTree.MainBranch;

            isAnalyzing = false;
            return new AnalyzeResponse(suggestions, playIndex, result);
        }

        public async Task<MoveSuggestion> AnalyzeMove(Move move)
        {
            if (isAnalyzing)
            {
                return null;
            }
            isAnalyzing = true;

            MoveSuggestion suggestion = (await GetKataGo()).AnalyzeMove(move);

            MoveTree.CurrentNode.Suggestions ??= new MoveSuggestionList();
            MoveTree.CurrentNode.Suggestions.AnalyzeMoveSuggestion = suggestion;

            isAnalyzing = false;
            return suggestion;
        }

        public async Task PlayPlayer(Move move, EPlayerResult playerResult, Coord? chosenNotPlayedCoord,
            int rightStreak, int perfectStreak, int? rightTopStreak, int? perfectTopStreak)
        {
            await Play(move, EMoveOrigin.PLAYER);

            MoveTree.CurrentNode.PlayerResult = playerResult;
            MoveTree.CurrentNode.Parent.ChosenNotPlayedCoord = chosenNotPlayedCoord;

            Game.RightStreak = rightStreak;
            Game.PerfectStreak = perfectStreak;
            Game.RightTopStreak = rightTopStreak != null ? rightTopStreak.Value : Game.RightTopStreak;
            Game.PerfectTopStreak = perfectTopStreak != null ? perfectTopStreak.Value : Game.PerfectTopStreak;
        }

        public async Task<MoveSuggestion> PlayForcedCorner(Move move)
        {
            MoveSuggestion suggestion = await AnalyzeMove(move);
            await Play(move, EMoveOrigin.FORCED_CORNER);
            return suggestion;
        }

        private void ApplyThirdPartyMoves(TreeNode<Move> treeNode)
        {
            MoveNode moveNode = MoveTree.Add(treeNode.Value);
            foreach (TreeNode<Move> childTreeNode in treeNode.Children)
            {
                ApplyThirdPartyMoves(childTreeNode);
                MoveTree.CurrentNode = moveNode;
            }
        }

        private double? GetResult(MoveSuggestionList moveSuggestionList)
        {
            MoveSuggestion? passSuggestion = moveSuggestionList.PassSuggestion;
            if (passSuggestion == null)
            {
                return null;
            }

            double scoreLead = passSuggestion.Score.ScoreLead;
            // Round to nearest x.0 or x.5
            scoreLead = Math.Round(scoreLead * 2, MidpointRounding.AwayFromZero) / 2;
            return Math.Round(scoreLead, 1);
        }

        private int? CalcPlayIndex(MoveSuggestionList suggestions, EMoveOrigin moveOrigin)
        {
            if (moveOrigin == EMoveOrigin.PLAYER)
            {
                return null;
            }

            int playIndex = 0;
            if (moveOrigin is EMoveOrigin.OPPONENT or EMoveOrigin.PRE)
            {
                if (!shouldBeImperfectSuggestion)
                {
                    if ((moveOrigin == EMoveOrigin.OPPONENT &&
                        TrainerSettingConfig.OpponentOptionPercSwitch &&
                        rnd.Next(1, 101) <= TrainerSettingConfig.OpponentOptionPerc)
                        ||
                        (moveOrigin == EMoveOrigin.PRE &&
                        TrainerSettingConfig.PreOptionPercSwitch &&
                        rnd.Next(1, 101) <= TrainerSettingConfig.PreOptionPerc))
                    {
                        shouldBeImperfectSuggestion = true;
                    }
                }

                if (shouldBeImperfectSuggestion)
                {
                    List<MoveSuggestion> imperfectSuggestions = suggestions.Suggestions.Where(s => s.Grade != "A").ToList();

                    if (imperfectSuggestions.Count != 0)
                    {
                        shouldBeImperfectSuggestion = false;

                        MoveSuggestion suggestion = imperfectSuggestions[rnd.Next(imperfectSuggestions.Count)];
                        for (int i = 0; i < suggestions.Suggestions.Count; i++)
                        {
                            if (suggestions.Suggestions[i] == suggestion)
                            {
                                playIndex = i;
                                break;
                            }
                        }
                    }
                }
            }

            return playIndex;
        }

        private async Task Play(Move move, EMoveOrigin moveOrigin)
        {
            MoveTree.Add(move);
            MoveTree.CurrentNode.MoveOrigin = moveOrigin;

            if (MoveTree.MainBranch == MoveTree.CurrentNode.Parent)
            {
                MoveTree.MainBranch = MoveTree.CurrentNode;
            }

            (await GetKataGo()).Play(move);
        }

        private async Task SyncBoard(Move[] moves)
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

        private async Task Save()
        {
            if (!MoveTree.AllNodes.Any(node => node.PlayerResult != null))
            {
                return;
            }

            if (Game.Id == null)
            {
                Game.GenerateId();
            }

            Game.UserId = UserId;
            Game.ProductVersion = await SG.GetVersion();
            Game.KataGoVersionId = (await pool.GetVersion()).Id;
            Game.Ruleset = TrainerSettingConfig.GetRuleset;
            Game.Komi = TrainerSettingConfig.GetKomi;

            SetMainBranchColor();

            await AddGameStats();

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            TrainerSettingConfig.SetHash();
            TrainerSettingConfig? duplicateTrainerSettingConfig = await dbContext.TrainerSettingConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Hash == TrainerSettingConfig.Hash);

            if (duplicateTrainerSettingConfig != null)
            {
                TrainerSettingConfig = duplicateTrainerSettingConfig;
            }
            else
            {
                TrainerSettingConfig.Id = 0;
                await dbContext.TrainerSettingConfigs.AddAsync(TrainerSettingConfig);
                await dbContext.SaveChangesAsync();
            }

            await dbContext.DisposeAsync();

            dbContext = await dbContextFactory.CreateDbContextAsync();

            Game.TrainerSettingConfigId = TrainerSettingConfig.Id;

            await SetGameName(dbContext);

            if (isExistingGame)
            {
                dbContext.Games.Update(Game);
            }
            else
            {
                await dbContext.Games.AddAsync(Game);
            }

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            dbContext = await dbContextFactory.CreateDbContextAsync();

            GameEncoder gameEncoder = new();
            byte[] data = gameEncoder.Encode(MoveTree);

            if (Game.EncodedGameData != null)
            {
                Game.EncodedGameData.Data = data;
                dbContext.Update(Game.EncodedGameData);
            }
            else
            {
                Game.EncodedGameData = new(Game.Id, data);
                await dbContext.EncodedGameDatas.AddAsync(Game.EncodedGameData);
            }

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        private void SetMainBranchColor()
        {
            if (Game.Color != EMoveColor.RANDOM)
            {
                return;
            }

            MoveNode parentNode = MoveTree.MainBranch ?? MoveTree.CurrentNode;
            while (parentNode != null)
            {
                if (parentNode.MoveOrigin == EMoveOrigin.PLAYER)
                {
                    Game.Color = parentNode.Move.Color.Value;
                    return;
                }

                parentNode = parentNode.Parent;
            }

            Game.Color = EMoveColor.BLACK;
            if (TrainerSettingConfig.ColorType != EMoveColor.RANDOM)
            {
                Game.Color = TrainerSettingConfig.ColorType;
            }
        }

        private async Task AddGameStats()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

            if (Game.GameStat != null)
            {
                dbContext.Remove(Game.GameStat);
            }
            if (Game.OpeningStat != null)
            {
                dbContext.Remove(Game.OpeningStat);
                Game.OpeningStat = null;
                Game.OpeningStatId = null;
            }
            if (Game.MidgameStat != null)
            {
                dbContext.Remove(Game.MidgameStat);
                Game.MidgameStat = null;
                Game.MidgameStatId = null;
            }
            if (Game.EndgameStat != null)
            {
                dbContext.Remove(Game.EndgameStat);
                Game.EndgameStat = null;
                Game.EndgameStatId = null;
            }

            MoveNode parentNode = MoveTree.MainBranch ?? MoveTree.CurrentNode;
            List<MoveNode> nodes = [];
            while (parentNode != null)
            {
                nodes.Add(parentNode);
                parentNode = parentNode.Parent;
            }

            nodes.Reverse();

            GameStat gameStat = new(1, nodes.Count);
            GameStat openingStat = new(1, Math.Min(nodes.Count, MIDGAME_MOVE_NUMBER - 1));
            GameStat midgameStat = new(MIDGAME_MOVE_NUMBER, Math.Min(nodes.Count, ENDGAME_MOVE_NUMBER - 1));
            GameStat endgameStat = new(ENDGAME_MOVE_NUMBER, nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                MoveNode node = nodes[i];
                EPlayerResult? playerResult = node.PlayerResult;
                if (playerResult == null)
                {
                    continue;
                }

                int moveNumber = i + 1;

                UpdateGameStatWithResult(gameStat, playerResult.Value);
                if (moveNumber < MIDGAME_MOVE_NUMBER)
                {
                    UpdateGameStatWithResult(openingStat, playerResult.Value);
                }
                else if (moveNumber < ENDGAME_MOVE_NUMBER)
                {
                    UpdateGameStatWithResult(midgameStat, playerResult.Value);
                }
                else
                {
                    UpdateGameStatWithResult(endgameStat, playerResult.Value);
                }
            }

            await dbContext.GameStats.AddAsync(gameStat);
            if (openingStat.Total != 0)
            {
                await dbContext.GameStats.AddAsync(openingStat);
            }
            if (midgameStat.Total != 0)
            {
                await dbContext.GameStats.AddAsync(midgameStat);
            }
            if (endgameStat.Total != 0)
            {
                await dbContext.GameStats.AddAsync(endgameStat);
            }

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            Game.GameStatId = gameStat.Id;
            Game.OpeningStatId = openingStat.Id != 0 ? openingStat.Id : null;
            Game.MidgameStatId = midgameStat.Id != 0 ? midgameStat.Id : null;
            Game.EndgameStatId = endgameStat.Id != 0 ? endgameStat.Id : null;
        }

        private void UpdateGameStatWithResult(GameStat stat, EPlayerResult playerResult)
        {
            stat.Total++;
            if (playerResult == EPlayerResult.PERFECT)
            {
                stat.Perfect++;
                stat.Right++;
            }
            else if (playerResult == EPlayerResult.RIGHT)
            {
                stat.Right++;
            }
        }

        private async Task SetGameName(ApplicationDbContext dbContext)
        {
            if (!string.IsNullOrEmpty(Game.Name))
            {
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Preset? preset = await dbContext.Presets
                    .Where(p => p.UserId == null || p.UserId == UserId)
                    .Where(p => p.TrainerSettingConfigId == TrainerSettingConfig.Id)
                    .FirstOrDefaultAsync();

                name = preset?.Name ?? Game.DEFAULT_NAME;
            }

            List<string> existingNames = await dbContext.Games
                .Where(g => g.UserId == UserId)
                .Select(g => g.Name)
                .Where(n => n.StartsWith(name))
                .ToListAsync();

            if (existingNames.Count != 0)
            {
                int counter = 2;
                string newName;
                do
                {
                    newName = $"{name} ({counter})";
                    counter++;
                } while (existingNames.Contains(newName));

                name = newName;
            }

            Game.Name = name;
        }

        private async Task StartKataGo()
        {
            await (await GetKataGo()).Restart();
            (await GetKataGo()).SetBoardsize(TrainerSettingConfig.Boardsize);
            (await GetKataGo()).SetRuleset(TrainerSettingConfig.GetRuleset);
            (await GetKataGo()).SetHandicap(TrainerSettingConfig.Handicap);
            (await GetKataGo()).SetKomi(TrainerSettingConfig.GetKomi);
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
            await pool.Return(UserId);
            //await Save();
        }
    }
}
