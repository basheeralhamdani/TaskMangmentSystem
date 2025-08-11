using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace TaskManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var updatedUser = await _userService.UpdateUserAsync(user);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> Authenticate([FromBody] LoginModel model)
        {
            var user = await _userService.AuthenticateUserAsync(model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordModel model)
        {
            var result = await _userService.ChangePasswordAsync(id, model.CurrentPassword, model.NewPassword);
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPost("assign-task")]
        public async Task<IActionResult> AssignTask([FromBody] TaskAssignmentModel model)
        {
            var result = await _userService.AssignTaskToUserAsync(model.TaskId, model.UserId);
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete("unassign-task/{taskId}")]
        public async Task<IActionResult> UnassignTask(Guid taskId)
        {
            var result = await _userService.UnassignTaskFromUserAsync(taskId);
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetUserTasks(Guid id)
        {
            var tasks = await _userService.GetUserAssignedTasksAsync(id);
            return Ok(tasks);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class TaskAssignmentModel
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
    }
}