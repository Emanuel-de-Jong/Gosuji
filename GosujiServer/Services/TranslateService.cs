using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;

namespace GosujiServer.Services
{
    public class TranslateService
    {
        private static Dictionary<long, Dictionary<string, string>>? TRANSLATIONS;

        private UserManager<User> userManager;
        private DbService dbService;
        private long languageId = 1;

        public TranslateService(UserManager<User> _userManager, DbService _dbService)
        {
            userManager = _userManager;
            dbService = _dbService;
        }

        public async Task Init(AuthenticationStateProvider authenticationStateProvider)
        {
            await Init((await authenticationStateProvider.GetAuthenticationStateAsync()).User);
        }

        public async Task Init(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                return;
            }

            await Init(await userManager.GetUserAsync(claimsPrincipal));
        }

        public async Task Init(User user)
        {
            await SetLanguageId(user);

            await Init();
        }

        public async Task Init()
        {
            if (TRANSLATIONS == null)
            {
                TRANSLATIONS = new();

                ApplicationDbContext dbContext = await dbService.GetContextAsync();

                foreach (TextValue val in dbContext.TextValues.Include(tv => tv.TextKey))
                {
                    if (!TRANSLATIONS.ContainsKey(val.LanguageId))
                    {
                        TRANSLATIONS[val.LanguageId] = new();
                    }

                    TRANSLATIONS[val.LanguageId][val.TextKey.Key] = val.Value;
                }

                await dbContext.DisposeAsync();
            }
        }

        private async Task SetLanguageId(User user)
        {
            if (user.SettingConfig == null)
            {
                ApplicationDbContext dbContext = await dbService.GetContextAsync();

                user.SettingConfig = await dbContext.SettingConfigs.FindAsync(user.SettingConfigId);

                await dbContext.DisposeAsync();
            }

            languageId = user.SettingConfig.LanguageId;
        }

        public string Get(string key)
        {
            return TRANSLATIONS[languageId][key];
        }
    }
}
