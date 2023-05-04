using Microsoft.AspNetCore.Mvc;

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
            var attachmentFile = _estimationService.GetEstimationAttachment(estimationId, attachmentType);

            if (attachmentFile == null)
            {
                return Problem("Couldn't load attachment");
            }

            var attachmentName = attachmentType == AttachmentType.Joints ? Constants.JOINTS_FILENAME : Constants.PREVIEW_FILENAME;
            return File(attachmentFile, "application/octet-stream", attachmentName);
        }
    }
}

