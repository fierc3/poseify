using Core.Models;
using Core.Services.Estimations;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EstimationController : ControllerBase
    {

        private readonly IEstimationService _estimationHandler;

        public EstimationController(IEstimationService estimationHandler)
        {
            _estimationHandler = estimationHandler;
        }
        

        [ActionName("GetUserEstimations")]
        [HttpGet(Name = "GetUserEstimations")]
        public ActionResult<IEnumerable<Estimation>?> Get()
        {
            
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
            {
                return Problem("Missing username, probably not logged in");
            }

            try
            {
                return _estimationHandler.GetAllUserEstimations(username).ToList();
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
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
            {
                return Problem("Missing username, probably not logged in");
            }

            try
            {
                _estimationHandler.DeleteEstimation(estimationId, username);
                return true;
            }
            catch (Exception ex)
            {
                return Problem($"A problem occured when trying to delete estimation:{ex}");
            }
        }
    }
}