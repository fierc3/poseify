using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UploadController : ControllerBase
    {

        private readonly ILogger<UploadController> _logger;
        private readonly IEstimationService _estimationService;
        private readonly IConfiguration _configuration;

        public UploadController(ILogger<UploadController> logger, IEstimationService estimationHandler, IConfiguration configuration)
        {
            _logger = logger;
            _estimationService = estimationHandler;
            _configuration = configuration;
        }

        public delegate void Notify();
        public event Notify UploadCompleted; // event

        [ActionName("PostUpload")]
        [HttpPost(Name = "PostUpload")]
        public async Task<IActionResult> PostUploadAsync([FromForm] UploadModel uploadModel)
        {
            if (uploadModel.FormFile == null)
            {
                return Problem("Missing Upload File");
            }

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(username == null)
            {
                return Problem("Missing username, probably not logged in");
            }

            var file = uploadModel.FormFile;

            string? directory = _configuration["UploadDirectory"];
            string userGuid = username;
            string uniqueFilename = Guid.NewGuid().ToString();
            string fileExtension = System.IO.Path.GetExtension(file.FileName);

            if (directory == null)
            {
                return Problem("Upload Directory is not defined");
            }

            if (file.Length == 0)
            {
                return Problem("File has zero size");
            }

            var maxSizeInMb = 50;

            if (file.Length > maxSizeInMb * 1048576)
            {
                return Problem($"File bigger than {maxSizeInMb}mb is not allowed");
            }

            if (file.Length > 0)
            {

                System.IO.Directory.CreateDirectory($"{directory}\\{userGuid}");

                string fileLocation = $"{directory}\\{userGuid}\\{uniqueFilename}{fileExtension}";

                using (var stream = System.IO.File.Create(fileLocation))
                {
                    await file.CopyToAsync(stream);
                }

                try
                {
                    _ = Task.Run(() => _estimationService.HandleUploadedFile(userGuid, directory, uniqueFilename, fileExtension, uploadModel.EstimationName, uploadModel.Tags));
                }
                catch (Exception ex)
                {
                    return Problem($"A problem occured when trying to convert input with VideoPose3D\nDetail:{ex}");
                }
                

            }

            return Ok(new { name = uniqueFilename });
        }
    }
}