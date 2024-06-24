using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gosuji.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
