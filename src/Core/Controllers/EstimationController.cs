using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EstimationController : ControllerBase
    {

        private readonly ILogger<EstimationController> _logger;
        private readonly IEstimationService _estimationHandler;
        private readonly IConfiguration _configuration;

        public EstimationController(ILogger<EstimationController> logger, IEstimationService estimationHandler, IConfiguration configuration)
        {
            _logger = logger;
            _estimationHandler = estimationHandler;
            _configuration = configuration;
        }
        
        [ActionName("GetEstimation")]
        [HttpGet(Name = "GetEstimation")]
        public ActionResult<Estimation> Get(string userGuid, string fileName, string fileExtension, string displayName, IEnumerable<string>? tags)
        {
            string? directory = _configuration["UploadDirectory"];

            if (directory == null)
            {
                return Problem("Upload Directory is not defined");
            }

            Estimation? estimation;
            try
            {
                estimation = _estimationHandler.HandleUploadedFile(userGuid, directory, fileName, fileExtension, displayName, null);
            }
            catch (Exception ex)
            {
                return Problem($"A problem occured when trying to convert input with VideoPose3D\nDetail:{ex}");
            }
            return estimation;
        }

        [ActionName("GetUserEstimations")]
        [HttpGet(Name = "GetUserEstimations")]
        public ActionResult<IEnumerable<Estimation>?> Get()
        {
            try
            {
                //todo use userid from token
                return _estimationHandler.GetAllUserEstimations("DEEZNUZ").ToList();
            }
            catch (Exception ex)
            {
                return Problem($"A problem occured when trying to fetch user uploads:{ex}");
            }
        }

        [ActionName("DeleteEstimation")]
        [HttpDelete(Name = "DeleteEstimation")]
        public ActionResult<bool> DeleteEstimation(string estimationId)
        {
            try
            {
                _estimationHandler.DeleteEstimation(estimationId);
                return true;
            }
            catch (Exception ex)
            {
                return Problem($"A problem occured when trying to delete estimation:{ex}");
            }
        }

        // ---- only for testing purposes ----

        [ActionName("GetEstimationTest")]
        [HttpGet(Name = "GetEstimationTest")]
        public ActionResult<Estimation> GetTest()
        {
            // assuming this can only happen if user exists in db, so user_id isnt being checked
            string userGuid = "DEEZNUZ";
            string fileName = "test_man";
            string fileExtension = "mp4";
            string displayName = "test1";
            string? directory = _configuration["UploadDirectory"];

            if (directory == null) {
                return Problem("Configuration issue, missing UploadDirectory");
            }

            Estimation? estimation;
            try
            {
                estimation = _estimationHandler.HandleUploadedFile(userGuid, directory, fileName, fileExtension, displayName, null);
            }
            catch (Exception ex)
            {
                return Problem($"A problem occured when trying to convert input with VideoPose3D\nDetail:{ex}");
            }
            return estimation;
        }
    }
}