using Gosuji.Client.Data;
using Gosuji.Client.ViewModels;

namespace Gosuji.Client.Services
{
    public interface IDataService
    {
        Task<Changelog[]> GetChangelogs();
        Task<Dictionary<string, long>> GetUserLanguageIds();
        Task<VMGame[]> GetUserGames(string userId);
        Task<Game?> GetGame(long gameId);
        Task<long> PostTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig);
        Task<long> PostGameStat(GameStat gameStat);
        Task PutGameStat(GameStat gameStat);
        Task<long> PostGame(Game game);
        Task PutGame(Game game);
        Task PostFeedback(Feedback feedback);
        Task<SettingConfig> GetSettingConfig(string userId);
        Task PutSettingConfig(SettingConfig settingConfig);
        Task<Dictionary<string, Language>> GetLanguages();
    }
}
