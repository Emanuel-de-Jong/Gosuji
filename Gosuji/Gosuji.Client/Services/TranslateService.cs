namespace Gosuji.Client.Services
{
    public class TranslateService : ITranslateService
    {
        private IDataService dataService;

        public bool IsInitialized { get; private set; }

        private Dictionary<long, Dictionary<string, string>> translations;
        private Dictionary<string, long> userLanguageIds;

        public TranslateService(IDataService _dataService)
        {
            dataService = _dataService;
        }

        public async Task Init()
        {
            if (IsInitialized)
            {
                return;
            }

            translations = await dataService.GetKeyValuesByLanguage();
            userLanguageIds = await dataService.GetUserLanguageIds();

            IsInitialized = true;
        }

        public string? Get(string? userId, string key)
        {
            long languageId = userId != null ? userLanguageIds[userId] : 1;

            if (!translations.ContainsKey(languageId))
            {
                return null;
            }

            if (!translations[languageId].ContainsKey(key))
            {
                return null;
            }

            return translations[languageId][key];
        }
    }
}
