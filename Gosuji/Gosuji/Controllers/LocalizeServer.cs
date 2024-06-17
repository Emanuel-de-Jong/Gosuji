using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Text.Json;

namespace Gosuji.Controllers
{
    public class LocationInfo
    {
        public string CountryCode { get; set; }
    }

    public class LocalizeServer
    {
        private static readonly string[] supportedCultures = { "en", "zh", "ko", "ja" };

        public static void Setup(WebApplication app)
        {
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.Use(DefaultCulturePicker);
        }

        private static async Task DefaultCulturePicker(HttpContext context, Func<Task> next)
        {
            IRequestCultureFeature? requestCulture = context.Features.Get<IRequestCultureFeature>();
            string defaultCulture = supportedCultures[0];

            if (requestCulture?.RequestCulture.Culture != null)
            {
                CultureInfo userLanguage = requestCulture.RequestCulture.Culture;
                if (supportedCultures.Contains(userLanguage.Name))
                {
                    defaultCulture = userLanguage.Name;
                }
            }
            else
            {
                // Use IP to determine location and set culture
                string? ip = context.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(ip))
                {
                    using HttpClient httpClient = new();
                    string response = await httpClient.GetStringAsync($"http://ip-api.com/json/{ip}");
                    LocationInfo? locationInfo = JsonSerializer.Deserialize<LocationInfo>(response);

                    if (locationInfo != null && !string.IsNullOrEmpty(locationInfo.CountryCode))
                    {
                        // Map country code to culture
                        defaultCulture = locationInfo.CountryCode switch
                        {
                            "US" => "en",
                            "CN" => "zh",
                            "KR" => "ko",
                            "JP" => "ja",
                            _ => defaultCulture,
                        };
                    }
                }
            }

            CultureInfo cultureInfo = new(defaultCulture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            await next.Invoke();
        }
    }
}
