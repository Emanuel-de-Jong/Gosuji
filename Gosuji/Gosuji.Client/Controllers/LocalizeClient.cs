using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace Gosuji.Client.Controllers
{
    public class LocalizeClient
    {
        public static async Task Setup(WebAssemblyHost host)
        {
            IJSRuntime js = host.Services.GetRequiredService<IJSRuntime>();
            string? result = await js.InvokeAsync<string>("utils.getCookie", "lang");
            CultureInfo culture = result != null ? CultureInfo.GetCultureInfo(result) : CultureInfo.CurrentCulture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
