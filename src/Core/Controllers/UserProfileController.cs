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

        [HttpPost(Name = "TestSavingAttachmentToDb")]
        public ActionResult<Estimation> Post(string guid)
        {
            _logger.LogDebug("Testing saving attachment to DB");
            Estimation? estimation = null;
            UserProfile? usr = null;
            // get session for raven db
            using (var session = _store.OpenSession())
            using (var file1 = System.IO.File.Open("..\\..\\..\\poseify\\Estimation_test\\Uploads\\test_input\\test_man.mp4.npz", FileMode.Open))
            {
                usr = session.Query<UserProfile>().Where(x => x.InternalGuid == guid).FirstOrDefault();
                string estimation_id = "estimation/1";
                estimation = new Estimation
                {
                    Guid = "Testimation1",
                    Tags = new List<string>(),
                    UploadingProfile = usr.InternalGuid,
                    UploadDate = DateTime.Now
                };
                session.Store(estimation, estimation_id);
                session.Advanced.Attachments.Store(estimation_id, "test_attach.npz", file1);
                session.SaveChanges();
            }

            if (estimation == null)
            {
                return NotFound();
            }

            return estimation;
        }
    }
}