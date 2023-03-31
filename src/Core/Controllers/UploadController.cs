using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using System.Collections.Generic;
using static Raven.Client.Constants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Controllers { 

    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {

        private readonly ILogger<UploadController> _logger;
        private readonly IEstimationHandler _estimationHandler;
        private readonly IConfiguration _configuration;

        public UploadController(ILogger<UploadController> logger, IEstimationHandler estimationHandler, IConfiguration configuration)
        {
            _logger = logger;
            _estimationHandler = estimationHandler;
            _configuration = configuration;
        }


        [HttpGet(Name = "GetEstimation")]
        public ActionResult<Estimation> Get(string userGuid, string fileName, string fileExtension, string displayName, IEnumerable<string>? tags)
        {
            // todo make sure user exists in db
            string? directory = _configuration["UploadDirectory"];
            Estimation estimation = _estimationHandler.HanldeUploadedFile(userGuid, directory, fileName, fileExtension, displayName, null);
            return estimation;
        }
    }
}