using Gosuji.Client.Data;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Gosuji.Client.ViewModels;

namespace Gosuji.Client.Services
{
    public class DataService
    {
        private static string MAP_GROUP = "/api/Data";

        private HttpClient http;

        public DataService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("Auth");
        }

        public async Task<APIResponse<Subscription?>> GetSubscription(bool includeDiscount = false)
        {
            return await HttpResponseHandler.Get<Subscription?>(http,
                $"{MAP_GROUP}/GetSubscription?includeDiscount={includeDiscount}");
        }

        public async Task<Changelog[]?> GetChangelogs()
        {
            return (await HttpResponseHandler.Get<Changelog[]>(http,
                $"{MAP_GROUP}/GetChangelogs")).Data;
        }

        public async Task<APIResponse<List<VMGame>>> GetUserGames(int start, int end)
        {
            return await HttpResponseHandler.Get<List<VMGame>>(http,
                $"{MAP_GROUP}/GetUserGames/{start}/{end}");
        }

        public async Task<APIResponse<Game>> GetGame(long gameId)
        {
            return await HttpResponseHandler.Get<Game>(http,
                $"{MAP_GROUP}/GetGame/{gameId}");
        }

        public async Task<APIResponse<TrainerSettingConfig>> GetTrainerSettingConfig(long configId)
        {
            return await HttpResponseHandler.Get<TrainerSettingConfig>(http,
                $"{MAP_GROUP}/GetTrainerSettingConfig/{configId}");
        }

        public async Task<APIResponse<long>> PostTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostTrainerSettingConfig", trainerSettingConfig);
        }

        public async Task<APIResponse<long>> PostGameStat(GameStat gameStat)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostGameStat", gameStat);
        }

        public async Task<APIResponse> PutGameStat(GameStat gameStat)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutGameStat", gameStat);
        }

        public async Task<APIResponse<long>> PostGame(Game game)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostGame", game);
        }

        public async Task<APIResponse> PutGame(Game game)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutGame", game);
        }

        public async Task<APIResponse> PostFeedback(Feedback feedback)
        {
            return await HttpResponseHandler.Post(http,
                $"{MAP_GROUP}/PostFeedback", feedback);
        }

        public async Task<APIResponse<SettingConfig>> GetSettingConfig()
        {
            return await HttpResponseHandler.Get<SettingConfig>(http,
                $"{MAP_GROUP}/GetSettingConfig");
        }

        public async Task<APIResponse> PutSettingConfig(SettingConfig settingConfig)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutSettingConfig", settingConfig);
        }

        public async Task<APIResponse<Dictionary<string, Language>>> GetLanguages()
        {
            return await HttpResponseHandler.Get<Dictionary<string, Language>>(http,
                $"{MAP_GROUP}/GetLanguages");
        }

        public async Task<APIResponse<Dictionary<long, Preset>>> GetPresets()
        {
            return await HttpResponseHandler.Get<Dictionary<long, Preset>>(http,
                $"{MAP_GROUP}/GetPresets");
        }

        public async Task<APIResponse<long>> PostPreset(Preset preset)
        {
            return await HttpResponseHandler.Post<long>(http,
                $"{MAP_GROUP}/PostPreset", preset);
        }

        public async Task<APIResponse> PutPreset(Preset preset)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutPreset", preset);
        }

        public async Task<APIResponse> DeletePreset(long presetId)
        {
            return await HttpResponseHandler.Delete(http,
                $"{MAP_GROUP}/DeletePreset/{presetId}");
        }

        public async Task<APIResponse<UserState>> GetUserState()
        {
            return await HttpResponseHandler.Get<UserState>(http,
                $"{MAP_GROUP}/GetUserState");
        }

        public async Task<APIResponse> PutUserState(UserState userState)
        {
            return await HttpResponseHandler.Put(http,
                $"{MAP_GROUP}/PutUserState", userState);
        }
    }
}
