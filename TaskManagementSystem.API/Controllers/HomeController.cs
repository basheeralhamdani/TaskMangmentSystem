using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.API.Models;
using System.Security.Claims;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;


namespace TaskManagementSystem.API.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<HomeController> _logger;

        private readonly IUserService _userService;

        public HomeController(ITaskService taskService, IUserService userService, ILogger<HomeController> logger)
        {
            _taskService = taskService;
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
                Enum.TryParse<UserRole>(userRoleStr, out var userRole);

                IEnumerable<TaskItem> scopeTasks;
                if (userRole == UserRole.SystemAdministrator || userRole == UserRole.Manager || userRole == UserRole.TaskAdministrator)
                {
                    scopeTasks = await _taskService.GetAllTasksAsync();
                }
                else
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    Guid.TryParse(idClaim, out var userId);
                    if (userId == Guid.Empty)
                    {
                        var username = User.Identity?.Name;
                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            var u = await _userService.GetUserByUsernameAsync(username);
                            if (u != null) userId = u.Id;
                        }
                    }
                    scopeTasks = await _taskService.GetTasksAssignedToUserAsync(userId);
                }

                var stats = new DashboardViewModel
                {
                    TotalTasks = scopeTasks.Count(),
                    CompletedTasks = scopeTasks.Count(t => t.Status == TaskManagementSystem.Core.Models.TaskStatus.Completed),
                    PendingTasks = scopeTasks.Count(t => t.Status == TaskManagementSystem.Core.Models.TaskStatus.Pending || t.Status == TaskManagementSystem.Core.Models.TaskStatus.InProgress),
                    HighPriorityTasks = scopeTasks.Count(t => t.Priority == PriorityLevel.High || t.Priority == PriorityLevel.Critical),
                    RecentTasks = scopeTasks.OrderByDescending(t => t.CreationDate).Take(5)
                };

                return View(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}