using Gosuji.Client.Data;
using System.Net.Http.Json;

namespace Gosuji.Client.Services
{
    public class ClientDataService(HttpClient http) : IDataService
    {
        private static string MAP_GROUP = "/api/DataService";

        public async Task<Changelog[]> GetChangelogs()
        {
            return await http.GetFromJsonAsync<Changelog[]>($"{MAP_GROUP}/GetChangelogs");
        }

        public async Task<Dictionary<long, Dictionary<string, string>>> GetKeyValuesByLanguage()
        {
            return await http.GetFromJsonAsync<Dictionary<long, Dictionary<string, string>>>($"{MAP_GROUP}/GetKeyValuesByLanguage");
        }

        public async Task<Dictionary<string, long>> GetUserLanguageIds()
        {
            return await http.GetFromJsonAsync<Dictionary<string, long>>($"{MAP_GROUP}/GetUserLanguageIds");
        }

        public async Task<Game[]> GetUserGames(string userId)
        {
           return await http.GetFromJsonAsync<Game[]>($"{MAP_GROUP}/GetUserGames/{userId}");
        }

        public async Task<Game?> GetGame(long gameId)
        {
            return await http.GetFromJsonAsync<Game?>($"{MAP_GROUP}/GetGame/{gameId}");
        }

        public async Task<long> PostTrainerSettingConfig(TrainerSettingConfig trainerSettingConfig)
        {
           return long.Parse(await (await http.PostAsJsonAsync($"{MAP_GROUP}/PostTrainerSettingConfig", trainerSettingConfig)).Content.ReadAsStringAsync());
        }

        public async Task<long> PostGameStat(GameStat gameStat)
        {
           return long.Parse(await (await http.PostAsJsonAsync($"{MAP_GROUP}/PostGameStat", gameStat)).Content.ReadAsStringAsync());
        }

        public async Task PutGameStat(GameStat gameStat)
        {
            await http.PostAsJsonAsync($"{MAP_GROUP}/PutGameStat", gameStat);
        }

        public async Task<long> PostGame(Game game)
        {
            return long.Parse(await (await http.PostAsJsonAsync($"{MAP_GROUP}/PostGame", game)).Content.ReadAsStringAsync());
        }

        public async Task PutGame(Game game)
        {
            await http.PostAsJsonAsync($"{MAP_GROUP}/PutGame", game);
        }
    }
}
