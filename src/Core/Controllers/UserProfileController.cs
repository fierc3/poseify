using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Controllers { 

    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {

        private readonly ILogger<UserProfileController> _logger;
        private readonly IDocumentStore _store;

        public UserProfileController(ILogger<UserProfileController> logger)
        {
            _logger = logger;
            // read ravendb store via lazy loading
            _store = DocumentStoreHolder.Store;
        }


        [HttpGet(Name = "GetCurrentUserProfile")]
        public ActionResult<UserProfile> Get(string guid)
        {
            _logger.LogDebug("UserProfile GET requesed");
            UserProfile? data = null;

            // get session for raven db
            using (var session = _store.OpenSession())
            {
                // use linq or rql to load Userprofile
                data = session.Query<UserProfile>().Where(x => x.InternalGuid == guid).FirstOrDefault();
            }

            if (data == null)
            {
                return NotFound();
            }

            return data;
        }
    }
}