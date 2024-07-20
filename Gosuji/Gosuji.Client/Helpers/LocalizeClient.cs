using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace Gosuji.Client.Helpers
{
    public class LocalizeClient
    {
        public static async Task Setup(WebAssemblyHost host)
        {
            CultureInfo culture = CultureInfo.CurrentCulture; // Just so it's not null. CurrentCulture is never used.

            IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
            string? localLang = await js.InvokeAsync<string>("utils.getLocal", G.LangLocalStorageName);
            if (localLang != null)
            {
                culture = CultureInfo.GetCultureInfo(localLang);
            }
            else
            {
                string? navigatorLang = await js.InvokeAsync<string>("eval", "navigator.language");
                culture = navigatorLang != null ? CultureInfo.GetCultureInfo(navigatorLang) : CultureInfo.GetCultureInfo(G.SupportedLangs[0]);
            }

            if (!G.SupportedLangs.Contains(culture.TwoLetterISOLanguageName))
            {
                culture = CultureInfo.GetCultureInfo(G.SupportedLangs[0]);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
