using Gosuji.Client.Data;
using Gosuji.Client.Helpers;
using Gosuji.Client.ViewModels;

namespace Gosuji.Client.Services
{
    public class DataService(HttpClient http)
    {
        private static string MAP_GROUP = "/api/Data";

        public async Task<Changelog[]?> GetChangelogs()
        {
            return await HttpResponseHandler.Get<Changelog[]>(http,
                $"{MAP_GROUP}/GetChangelogs");
        }

        public async Task<List<VMGame>?> GetUserGames(int start, int end)
        {
            return await HttpResponseHandler.Get<List<VMGame>>(http,
                $"{MAP_GROUP}/GetUserGames/{start}/{end}");
        }

        public async Task<Game?> GetGame(long gameId)
        {
            return await HttpResponseHandler.Get<Game>(http,
                $"{MAP_GROUP}/GetGame/{gameId}");
        }

        public async Task<TrainerSettingConfig?> GetTrainerSettingConfig(long configId)
        {
            return await HttpResponseHandler.Get<TrainerSettingConfig>(http,
                $"{MAP_GROUP}/GetTrainerSettingConfig/{configId}");
        }

        public async Task<long?> PostTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostTrainerSettingConfig", trainerSettingConfig);
        }

        public async Task<long?> PostGameStat(GameStat gameStat)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostGameStat", gameStat);
        }

        public async Task<bool> PutGameStat(GameStat gameStat)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutGameStat", gameStat);
        }

        public async Task<long?> PostGame(Game game)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostGame", game);
        }

        public async Task<bool> PutGame(Game game)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutGame", game);
        }

        public async Task<bool> PostFeedback(Feedback feedback)
        {
            return await HttpResponseHandler.Post(http,
                $"{MAP_GROUP}/PostFeedback", feedback);
        }

        public async Task<SettingConfig?> GetSettingConfig()
        {
            return await HttpResponseHandler.Get<SettingConfig>(http,
                $"{MAP_GROUP}/GetSettingConfig");
        }

        public async Task<bool> PutSettingConfig(SettingConfig settingConfig)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutSettingConfig", settingConfig);
        }

        public async Task<Dictionary<string, Language>?> GetLanguages()
        {
            return await HttpResponseHandler.Get<Dictionary<string, Language>>(http,
                $"{MAP_GROUP}/GetLanguages");
        }

        public async Task<List<Preset>?> GetPresets()
        {
            return await HttpResponseHandler.Get<List<Preset>>(http,
                $"{MAP_GROUP}/GetPresets");
        }

        public async Task<long?> PostPreset(Preset preset)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostPreset", preset);
        }

        public async Task<bool> PutPreset(Preset preset)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutPreset", preset);
        }

        public async Task<bool> DeletePreset(long presetId)
        {
            return await HttpResponseHandler.Delete(http,
                $"{MAP_GROUP}/DeletePreset/{presetId}");
        }
    }
}
