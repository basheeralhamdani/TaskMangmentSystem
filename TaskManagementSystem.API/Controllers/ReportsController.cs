using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public ReportsController(ITaskService taskService, IUserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get user ID and role from claims
                var userId = await ResolveCurrentUserIdAsync();
                var userRole = GetCurrentUserRole();
                IEnumerable<TaskItem> allTasks;

                // Filter tasks based on user role
                if (userRole == UserRole.Manager || userRole == UserRole.TaskAdministrator || userRole == UserRole.SystemAdministrator)
                {
                    allTasks = await _taskService.GetAllTasksAsync();
                }
                else
                {
                    allTasks = await _taskService.GetTasksAssignedToUserAsync(userId);
                }

                var viewModel = new TaskManagementSystem.API.Models.ReportsViewModel
                {
                    TotalTasks = allTasks.Count(),
                    CompletedTasks = allTasks.Count(t => t.Status == Core.Models.TaskStatus.Completed),
                    PendingTasks = allTasks.Count(t => t.Status == Core.Models.TaskStatus.Pending || t.Status == Core.Models.TaskStatus.InProgress),
                    HighPriorityTasks = allTasks.Count(t => t.Priority == PriorityLevel.High || t.Priority == PriorityLevel.Critical),
                    TasksByStatus = GetTasksByStatus(allTasks),
                    TasksByPriority = GetTasksByPriority(allTasks)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TaskStatistics(string? systemName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                // Get user ID and role from claims
                var userId = await ResolveCurrentUserIdAsync();
                var userRole = GetCurrentUserRole();
                IEnumerable<TaskItem> allTasks;

                // Filter tasks based on user role
                if (userRole == UserRole.Manager || userRole == UserRole.TaskAdministrator || userRole == UserRole.SystemAdministrator)
                {
                    allTasks = await _taskService.GetAllTasksAsync();
                }
                else
                {
                    allTasks = await _taskService.GetTasksAssignedToUserAsync(userId);
                }

                // Apply filters
                if (!string.IsNullOrEmpty(systemName))
                {
                    allTasks = allTasks.Where(t => t.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
                }

                if (startDate.HasValue)
                {
                    allTasks = allTasks.Where(t => t.CreationDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    allTasks = allTasks.Where(t => t.CreationDate <= endDate.Value);
                }

                var viewModel = new TaskManagementSystem.API.Models.TaskStatisticsViewModel
                {
                    TotalTasks = allTasks.Count(),
                    CompletedTasks = allTasks.Count(t => t.Status == Core.Models.TaskStatus.Completed),
                    PendingTasks = allTasks.Count(t => t.Status == Core.Models.TaskStatus.Pending || t.Status == Core.Models.TaskStatus.InProgress),
                    HighPriorityTasks = allTasks.Count(t => t.Priority == PriorityLevel.High || t.Priority == PriorityLevel.Critical),
                    TasksByStatus = GetTasksByStatus(allTasks),
                    TasksByPriority = GetTasksByPriority(allTasks),
                    TasksBySystem = GetTasksBySystem(allTasks),
                    TasksByDate = GetTasksByDate(allTasks)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }
        private async Task<Guid> ResolveCurrentUserIdAsync()
        {
            // First try NameIdentifier claim
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(claim, out var id)) return id;

            // Fallback: try username from Name claim, then look up user id
            var username = User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(username))
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user != null) return user.Id;
            }
            return Guid.Empty;
        }

        private Dictionary<Core.Models.TaskStatus, int> GetTasksByStatus(IEnumerable<TaskItem> tasks)
        {
            return Enum.GetValues(typeof(Core.Models.TaskStatus))
                .Cast<Core.Models.TaskStatus>()
                .ToDictionary(status => status, status => tasks.Count(t => t.Status == status));
        }

        private Dictionary<PriorityLevel, int> GetTasksByPriority(IEnumerable<TaskItem> tasks)
        {
            return Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .ToDictionary(priority => priority, priority => tasks.Count(t => t.Priority == priority));
        }

        private Dictionary<string, int> GetTasksBySystem(IEnumerable<TaskItem> tasks)
        {
            return tasks
                .GroupBy(t => t.SystemName)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private Dictionary<DateTime, int> GetTasksByDate(IEnumerable<TaskItem> tasks)
        {
            return tasks
                .GroupBy(t => t.CreationDate.Date)
                .OrderBy(group => group.Key)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(claim, out var id)) return id;
            return Guid.Empty;
        }

        private UserRole GetCurrentUserRole()
        {
            var roleString = User.FindFirst(ClaimTypes.Role)?.Value;
            if (Enum.TryParse<UserRole>(roleString, out var role)) return role;
            return UserRole.User;
        }
    }


}