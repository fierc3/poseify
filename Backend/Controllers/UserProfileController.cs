using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers { 

    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {

        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(ILogger<UserProfileController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetCurrentUserProfile")]
        public DefaultReturn<UserProfile, DefaultError> Get()
        {
            _logger.LogDebug("UserProfile GET requesed");
            var data = new UserProfile() { DisplayName = "John Doe", InternalGuid = "Secret", Token = "xxxxxxx", ImageUrl = "https://64.media.tumblr.com/06cdcf5cbf141c01e6e926ab4588c755/tumblr_navzwsvEMm1rdr926o1_400.gif" };
            var error = new DefaultError();
            return new DefaultReturn<UserProfile, DefaultError>()
            {
                Data = data,
                Error = error,
                Success = true
            };
        }
    }
}