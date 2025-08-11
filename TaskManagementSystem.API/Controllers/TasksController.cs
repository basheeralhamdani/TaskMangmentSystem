using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Core.Interfaces;

using TaskManagementSystem.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TaskManagementSystem.API.Models;


using System.Security.Claims;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, IUserService userService, INotificationService notificationService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _userService = userService;
            _notificationService = notificationService;
            _logger = logger;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<TaskItem> tasks;
                var roleStr = User.FindFirst(ClaimTypes.Role)?.Value;
                Enum.TryParse<UserRole>(roleStr, out var role);
                if (role == UserRole.SystemAdministrator || role == UserRole.Manager || role == UserRole.TaskAdministrator)
                {
                    tasks = await _taskService.GetAllTasksAsync();
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
                    tasks = await _taskService.GetTasksAssignedToUserAsync(userId);
                }
                var taskViewModels = tasks.Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    Status = t.Status,
                    SystemName = t.SystemName,
                    CreationDate = t.CreationDate,
                    DueDate = t.DueDate,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUsername = t.AssignedToUser?.Username ?? "Unassigned"
                }).ToList();

                return View(taskViewModels);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var comments = await _taskService.GetTaskCommentsAsync(task.Id);

                var taskViewModel = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority,
                    Status = task.Status,
                    SystemName = task.SystemName,
                    CreationDate = task.CreationDate,
                    DueDate = task.DueDate,
                    AssignedToUserId = task.AssignedToUserId,
                    AssignedToUsername = task.AssignedToUser?.Username ?? "Unassigned",
                    Comments = comments.ToList()
                };

                return View(taskViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(TaskCommentInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Details), new { id = input.TaskId });
            }

            try
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Guid.TryParse(idClaim, out var userId);
                if (userId == Guid.Empty)
                {
                    // Fallback: try username
                    var username = User.Identity?.Name;
                    var user = !string.IsNullOrWhiteSpace(username) ? await _userService.GetUserByUsernameAsync(username) : null;
                    if (user != null) userId = user.Id;
                }

                if (userId == Guid.Empty)
                {
                    return Forbid();
                }

                var ok = await _taskService.AddCommentToTaskAsync(input.TaskId, input.Content, userId);
                if (!ok)
                {
                    TempData["ErrorMessage"] = "Failed to add comment.";
                }
                else
                {
                    // Fetch task and notify relevant parties
                    var task = await _taskService.GetTaskByIdAsync(input.TaskId);
                    if (task != null)
                    {
                        // Notify assigned user (if commenter is not the assignee)
                        if (task.AssignedToUserId.HasValue)
                        {
                            var assignee = await _userService.GetUserByIdAsync(task.AssignedToUserId.Value);
                            if (assignee != null && assignee.Id != userId)
                            {
                                await _notificationService.SendCommentAddedNotificationAsync(assignee, task, new TaskManagementSystem.Core.Models.TaskComment
                                {
                                    TaskId = input.TaskId,
                                    UserId = userId,
                                    Content = input.Content,
                                    CreatedAt = DateTime.UtcNow
                                });
                            }
                        }
                    }
                }

                return RedirectToAction(nameof(Details), new { id = input.TaskId });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Guid id, TaskManagementSystem.Core.Models.TaskStatus newStatus)
        {
            try
            {
                var ok = await _taskService.ChangeTaskStatusAsync(id, newStatus);
                if (!ok)
                {
                    TempData["ErrorMessage"] = "Failed to change status.";
                }
                else
                {
                    // Notify assigned user if exists
                    var task = await _taskService.GetTaskByIdAsync(id);
                    if (task?.AssignedToUserId != null)
                    {
                        var user = await _userService.GetUserByIdAsync(task.AssignedToUserId.Value);
                        if (user != null)
                        {
                            await _notificationService.SendTaskStatusChangedNotificationAsync(user, task);
                        }
                    }
                }
                return RedirectToAction(nameof(Details), new { id });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Tasks/Create
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Create()
        {
            var users = await _userService.GetAllUsersAsync();
            ViewBag.Users = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Username} ({u.Email})"
            }).ToList();

            var vm = new TaskViewModel
            {
                Priority = PriorityLevel.Medium,
                Status = TaskManagementSystem.Core.Models.TaskStatus.Pending
            };
            return View(vm);
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Create(TaskViewModel taskViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var task = new TaskItem
                    {
                        Id = Guid.NewGuid(),
                        Title = taskViewModel.Title?.Trim(),
                        Description = taskViewModel.Description?.Trim(),
                        Priority = taskViewModel.Priority,
                        Status = taskViewModel.Status,
                        SystemName = taskViewModel.SystemName?.Trim(),
                        CreationDate = DateTime.UtcNow,
                        DueDate = taskViewModel.DueDate,
                        AssignedToUserId = taskViewModel.AssignedToUserId
                    };

                    var created = await _taskService.CreateTaskAsync(task);
                    _logger.LogInformation("Task created with Id {TaskId}", created.Id);

                    if (created.AssignedToUserId.HasValue)
                    {
                        var assignedUser = await _userService.GetUserByIdAsync(created.AssignedToUserId.Value);
                        if (assignedUser != null)
                        {
                            await _notificationService.SendTaskAssignedNotificationAsync(assignedUser, created);
                        }
                    }

                    if (taskViewModel.CreateAnother)
                    {
                        TempData["SuccessMessage"] = "Task created. You can create another.";
                        return RedirectToAction(nameof(Create));
                    }

                    return RedirectToAction("Dashboard", "Home");
                }

                _logger.LogWarning("Create Task ModelState invalid: {Errors}", string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

                var users = await _userService.GetAllUsersAsync();
                ViewBag.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Username} ({u.Email})"
                }).ToList();
                return View(taskViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Unable to create task. " + ex.Message);
                return View(taskViewModel);
            }
        }

        // GET: Tasks/Edit/5
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var taskViewModel = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority,
                    Status = task.Status,
                    SystemName = task.SystemName,
                    CreationDate = task.CreationDate,
                    DueDate = task.DueDate,
                    AssignedToUserId = task.AssignedToUserId
                };

                var users = await _userService.GetAllUsersAsync();
                ViewBag.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Username} ({u.Email})"
                }).ToList();

                return View(taskViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Edit(Guid id, TaskViewModel taskViewModel)
        {
            try
            {
                if (id != taskViewModel.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var task = new TaskItem
                    {
                        Id = taskViewModel.Id,
                        Title = taskViewModel.Title,
                        Description = taskViewModel.Description,
                        Priority = taskViewModel.Priority,
                        Status = taskViewModel.Status,
                        SystemName = taskViewModel.SystemName,
                        CreationDate = taskViewModel.CreationDate,
                        DueDate = taskViewModel.DueDate,
                        AssignedToUserId = taskViewModel.AssignedToUserId
                    };

                    await _taskService.UpdateTaskAsync(task);
                    return RedirectToAction(nameof(Index));
                }

                var users = await _userService.GetAllUsersAsync();
                ViewBag.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Username} ({u.Email})"
                }).ToList();
                return View(taskViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Unable to update task. " + ex.Message);
                return View(taskViewModel);
            }
        }

        // GET: Tasks/Delete/5
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var taskViewModel = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority,
                    Status = task.Status,
                    SystemName = task.SystemName,
                    CreationDate = task.CreationDate,
                    DueDate = task.DueDate,
                    AssignedToUserId = task.AssignedToUserId,
                    AssignedToUsername = task.AssignedToUser?.Username ?? "Unassigned"
                };

                return View(taskViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }
    }
}