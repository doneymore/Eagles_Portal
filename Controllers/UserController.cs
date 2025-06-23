using Eagles_Portal.contracts.Interface;
using Eagles_Portal.contracts.services;
using Eagles_Portal.DTO;
using Eagles_Portal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eagles_Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _services;

        public UserController(IUserServices service)
        {
            _services = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            var users = await _services.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            var user = await _services.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult<Users>> AddUser(Users user)
        {
            await _services.AddUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserResponseDTOcs updatedUser)
        {
            // It's good practice for the ID in the route to match the ID in the body for PUT requests.
            if (id != updatedUser.Id)
                return BadRequest("User ID mismatch.");

            var updateSuccessful = await _services.UpdateUserPartialAsync(id, updatedUser); // This returns bool
            if (!updateSuccessful) // Check the boolean result directly
                return NotFound();

            return NoContent(); // 204 No Content for successful update with no content to return
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id) // Change return type to IActionResult
        {
            var deleteSuccessful = await _services.DeleteUserAsync(id); // This returns bool
            if (!deleteSuccessful)
                return NotFound(); // Return 404 if user not found for deletion

            return NoContent(); // Return 204 No Content for successful deletion
        }
    }
}
