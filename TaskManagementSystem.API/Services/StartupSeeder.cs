using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Services
{
    public static class StartupSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var users = await userRepo.GetAllAsync();
            if (!users.Any(u => u.Role == UserRole.SystemAdministrator))
            {
                await userRepo.AddAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "Admin123!",
                    Role = UserRole.SystemAdministrator,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}
