using Gosuji.Client.Data;

namespace Gosuji.Client.Services
{
    public interface IDataService
    {
        Task<Changelog[]> GetChangelogs();
        Task<Dictionary<long, Dictionary<string, string>>> GetKeyValuesByLanguage();
        Task<Dictionary<string, long>> GetUserLanguageIds();
        Task<Game[]> GetUserGames(string userId);
        Task<Game?> GetGame(long gameId);
        Task<long> PostTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig);
        Task<long> PostGameStat(GameStat gameStat);
        Task PutGameStat(GameStat gameStat);
        Task<long> PostGame(Game game);
        Task PutGame(Game game);
        Task PostFeedback(Feedback feedback);
    }
}
