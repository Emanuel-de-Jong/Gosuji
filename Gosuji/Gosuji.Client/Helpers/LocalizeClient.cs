using Gosuji.Client.Data;
using Gosuji.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace Gosuji.Client.Helpers
{
    public class LocalizeClient
    {
        public static async Task Setup(WebAssemblyHost host)
        {
            ELanguage lang = ELanguage.en;

            IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
            string? localLang = await js.InvokeAsync<string>("utils.getLocal", SettingConfigService.LANGUAGE_ID_STORAGE_NAME);
            if (localLang != null)
            {
                Enum.TryParse(localLang, out lang);
            }
            else
            {
                string? navigatorLang = await js.InvokeAsync<string>("eval", "navigator.language");
                if (!string.IsNullOrWhiteSpace(navigatorLang))
                {
                    CultureInfo tempCulture = CultureInfo.GetCultureInfo(navigatorLang);

                    foreach (ELanguage supportedLang in Enum.GetValues(typeof(ELanguage)))
                    {
                        if (tempCulture.TwoLetterISOLanguageName.Contains(supportedLang.ToString()))
                        {
                            lang = supportedLang;
                            break;
                        }
                    }
                }
            }

            CultureInfo culture = CultureInfo.GetCultureInfo(lang.ToString());
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
