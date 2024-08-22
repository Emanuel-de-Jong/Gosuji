using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace Gosuji.Client.Helpers
{
    public class LocalizeClient
    {
        public static string[] SUPPORTED_LANGUAGES = { "en", "zh", "ko", "ja" };

        public static async Task Setup(WebAssemblyHost host)
        {
            CultureInfo culture = CultureInfo.CurrentCulture; // Just so it's not null. CurrentCulture is never used.

            IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
            string? localLang = await js.InvokeAsync<string>("utils.getLocal", SettingConfigService.LANGUAGE_ID_STORAGE_NAME);
            if (localLang != null)
            {
                culture = CultureInfo.GetCultureInfo(localLang);
            }
            else
            {
                string? navigatorLang = await js.InvokeAsync<string>("eval", "navigator.language");
                culture = navigatorLang != null ? CultureInfo.GetCultureInfo(navigatorLang) : CultureInfo.GetCultureInfo(SUPPORTED_LANGUAGES[0]);
            }

            if (!SUPPORTED_LANGUAGES.Contains(culture.TwoLetterISOLanguageName))
            {
                culture = CultureInfo.GetCultureInfo(SUPPORTED_LANGUAGES[0]);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
