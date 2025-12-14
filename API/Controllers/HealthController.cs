using API.Models;
using Application.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace FCG.FiapCloudGames.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// returns the current health status of the api.
        /// </summary>
        /// <returns>No content</returns>
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(Name = "Health")]
        public IActionResult Health()
        {
            return Ok("Healthy");
        }
    }
}
