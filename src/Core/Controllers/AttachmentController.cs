using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AttachmentController : ControllerBase
    {

        private readonly ILogger<AttachmentController> _logger;
        private readonly IEstimationService _estimationService;
        private readonly IConfiguration _configuration;

        public AttachmentController(ILogger<AttachmentController> logger, IEstimationService estimationHandler, IConfiguration configuration)
        {
            _logger = logger;
            _estimationService = estimationHandler;
            _configuration = configuration;
        }

        [ActionName("GetAttachment")]
        [HttpGet(Name = "GetAttachment")]
        public async Task<IActionResult> Get(string estimationId, AttachmentType attachmentType)
        {

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
            {
                return Problem("Missing username, probably not logged in");
            }

            var attachmentFile = _estimationService.GetEstimationAttachment(estimationId, attachmentType, username);

            if (attachmentFile == null)
            {
                return Problem("Couldn't load attachment");
            }

            return File(attachmentFile, "application/octet-stream", Constants.GetFilename(attachmentType));
        }
    }
}

