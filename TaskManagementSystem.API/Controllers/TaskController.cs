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
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetById(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create(TaskItem task)
        {
            var createdTask = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TaskItem task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            var updatedTask = await _taskService.UpdateTaskAsync(task);
            if (updatedTask == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                Total = await _taskService.GetTotalTasksAsync(),
                Completed = await _taskService.GetCompletedTasksAsync(),
                Pending = await _taskService.GetPendingTasksAsync(),
                HighPriority = await _taskService.GetHighPriorityTasksAsync()
            };

            return Ok(stats);
        }
    }
}