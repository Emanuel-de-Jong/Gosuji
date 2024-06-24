using Gosuji.Client;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.Controllers
{
    [Route("[controller]/[action]")]
    [EnableRateLimiting(G.ControllerRateLimitPolicyName)]
    public class CultureController : Controller
    {
        public IActionResult Set([Required, MinLength(1), MaxLength(100)] string culture,
            [Required, MinLength(1), MaxLength(100)] string redirectUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture, culture)));
            }

            return LocalRedirect(redirectUri);
        }
    }
}