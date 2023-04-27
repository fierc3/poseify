using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [ActionName("GetIdToken")]
        [HttpGet(Name = "GetIdToken")]
        public IActionResult GetIdToken()
        {
            // hint: "sub" claim name to "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            var name = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(name);
        }
    }
}