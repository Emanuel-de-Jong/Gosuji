using Gosuji.Client.Data;
using Gosuji.Client.Models.KataGo;
using Microsoft.JSInterop;

namespace Gosuji.Client.Services
{
    public interface IKataGoService
    {
        Task<KataGoVersion> GetVersion();
        Task Return(string userId);
        Task<bool> UserHasInstance(string userId);
        [JSInvokable]
        Task ClearBoard(string userId);
        [JSInvokable]
        Task Restart(string userId);
        [JSInvokable]
        Task SetBoardsize(string userId, int boardsize);
        [JSInvokable]
        Task SetRuleset(string userId, string ruleset);
        [JSInvokable]
        Task SetKomi(string userId, float komi);
        [JSInvokable]
        Task SetHandicap(string userId, int handicap);
        [JSInvokable]
        Task<MoveSuggestion> AnalyzeMove(string userId, string color, string coord);
        [JSInvokable]
        Task<List<MoveSuggestion>> Analyze(string userId, string color, int maxVisits, float minVisitsPerc, float maxVisitDiffPerc);
        [JSInvokable]
        Task Play(string userId, string color, string coord);
        [JSInvokable]
        Task PlayRange(string userId, Moves moves);
        [JSInvokable]
        Task<string> SGF(string userId, bool shouldWriteFile);
    }
}
