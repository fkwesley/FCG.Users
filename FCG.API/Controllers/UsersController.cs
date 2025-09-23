using FCG.API.Models;
using FCG.Application.DTO.User;
using FCG.Application.Interfaces;
using FCG.FiapCloudGames.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.FiapCloudGames.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        #region GETS
        /// <summary>
        /// Returns all users registered.
        /// </summary>
        /// <returns>List of Users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            var games = _userService.GetAllUsers();
            return Ok(games);
        }

        /// <summary>
        /// returns a user by id.
        /// </summary>
        /// <returns>Object User</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(string id)
        {
            var game = _userService.GetUserById(id);
            return Ok(game);
        }
        #endregion

        #region POST
        /// <summary>
        /// Add a user.
        /// </summary>
        /// <returns>Object User added</returns>
        [HttpPost(Name = "Users")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Add([FromBody] UserRequest user)
        {
            if (user.Name.ToLower() == "error 500 fake")
                throw new Exception("Error 500 adding user. [FAKE] ");

            var created = _userService.AddUser(user);
            return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
        }
        #endregion

        #region PUT
        /// <summary>
        /// Update a user.
        /// </summary>
        /// <returns>Object User updated</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Update(string id, [FromBody] UserRequest user)
        {
            user.UserId = id;
            var updated = _userService.UpdateUser(user);
            return Ok(updated);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(string id)
        {
            var deleted = _userService.DeleteUser(id);
            return NoContent();
        }
        #endregion
    }
}
