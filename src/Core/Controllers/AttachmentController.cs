using Core.Models;
using Core.Services.Estimations;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AttachmentController : ControllerBase
    {

        private readonly IEstimationService _estimationService;

        public AttachmentController(IEstimationService estimationHandler)
        {
            _estimationService = estimationHandler;
        }

        [ActionName("GetAttachment")]
        [HttpGet(Name = "GetAttachment")]
        public IActionResult Get(string estimationId, AttachmentType attachmentType)
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

