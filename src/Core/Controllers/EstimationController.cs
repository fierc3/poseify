using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using System.Collections.Generic;
using static Raven.Client.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Controllers { 

    [ApiController]
    [Route("[controller]")]
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

        [HttpGet(Name = "GetEstimation")]
        public ActionResult<Estimation> Get(string userGuid, string fileName, string fileExtension, string displayName, IEnumerable<string>? tags)
        {
            // assuming can only be called if user exists in db, so user_id isnt being checked
            string? directory = _configuration["UploadDirectory"];
            Estimation? estimation;
            try
            {
                estimation = _estimationHandler.HanldeUploadedFile(userGuid, directory, fileName, fileExtension, displayName, null);
            }
            catch (Exception ex)
            {
                return Problem($"A problem occured when trying to convert input with VideoPose3D\nDetail:{ex}");
            }
            return estimation;
        }

        // ---- only for testing purposes ----

        //[HttpGet(Name = "GetEstimation")]
        //public ActionResult<Estimation> Get()
        //{
        //    // assuming this can only happen if user exists in db, so user_id isnt being checked
        //    string userGuid = "DEEZNUZ";
        //    string fileName = "test_man";
        //    string fileExtension = "mp4";
        //    string displayName = "test1";
        //    string? directory = _configuration["UploadDirectory"];
        //    Estimation? estimation;
        //    try
        //    {
        //        estimation = _estimationHandler.HanldeUploadedFile(userGuid, directory, fileName, fileExtension, displayName, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Problem($"A problem occured when trying to convert input with VideoPose3D\nDetail:{ex}");
        //    }
        //    return estimation;
        //}
    }
}