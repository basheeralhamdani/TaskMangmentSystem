using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;

namespace TaskManagementSystem.API.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            Guid userId = Guid.Empty;
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(idClaim, out userId);
            if (userId == Guid.Empty)
            {
                var username = User.Identity?.Name;
                if (!string.IsNullOrWhiteSpace(username))
                {
                    var u = await _userService.GetUserByUsernameAsync(username);
                    if (u != null) userId = u.Id;
                }
            }

            if (userId == Guid.Empty)
            {
                return RedirectToAction("Error", "Home");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            return View(user);
        }
    }
}

