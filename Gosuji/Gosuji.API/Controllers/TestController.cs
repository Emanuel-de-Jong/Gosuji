using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gosuji.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : CustomControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Test2()
        {
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Test3()
        {
            return Ok();
        }
    }
}
