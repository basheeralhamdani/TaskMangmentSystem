using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize(Roles = "SystemAdministrator")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userViewModels = users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    AssignedTasksCount = u.AssignedTasks?.Count ?? 0
                }).ToList();

                return View(userViewModels);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    AssignedTasksCount = user.AssignedTasks?.Count ?? 0
                };

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Users/Create
        [Authorize(Roles = "TaskAdministrator, SystemAdministrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = userViewModel.Username,
                        Email = userViewModel.Email,
                        PasswordHash = userViewModel.PasswordHash, // In a real app, this would be hashed
                        Role = userViewModel.Role,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userService.CreateUserAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Unable to create user. " + ex.Message);
                return View(userViewModel);
            }
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin
                };

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TaskAdministrator, SystemAdministrator")]
        public async Task<IActionResult> Edit(Guid id, UserViewModel userViewModel)
        {
            try
            {
                if (id != userViewModel.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var user = new User
                    {
                        Id = userViewModel.Id,
                        Username = userViewModel.Username,
                        Email = userViewModel.Email,
                        Role = userViewModel.Role,
                        CreatedAt = userViewModel.CreatedAt,
                        LastLogin = userViewModel.LastLogin
                    };

                    await _userService.UpdateUserAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Unable to update user. " + ex.Message);
                return View(userViewModel);
            }
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    AssignedTasksCount = user.AssignedTasks?.Count ?? 0
                };

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
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