using Duende.Bff;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BffWeb.Controllers
{
    [Route("bff/[controller]/[action]")]
    [BffApi]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [ActionName("GetIdToken")]
        [HttpGet(Name = "GetIdToken")]
        public async Task<IActionResult> GetIdToken()
        {
            return new JsonResult(from p in (await HttpContext.AuthenticateAsync()).Properties?.Items select new { p.Key, p.Value });
        }
    }
}