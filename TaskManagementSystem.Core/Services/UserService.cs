using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Check if username already exists
            var existingUser = await GetUserByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            // Ensure exactly one SystemAdministrator in the system
            if (user.Role == UserRole.SystemAdministrator)
            {
                var all = await _userRepository.GetAllAsync();
                if (all.Any(u => u.Role == UserRole.SystemAdministrator))
                    throw new InvalidOperationException("There can only be one System Administrator.");
            }

            return await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // Check if username already exists (excluding current user)
            var existingUser = await GetUserByUsernameAsync(user.Username);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            // Enforce only one SystemAdministrator
            if (user.Role == UserRole.SystemAdministrator)
            {
                var all = await _userRepository.GetAllAsync();
                if (all.Any(u => u.Role == UserRole.SystemAdministrator && u.Id != user.Id))
                    throw new InvalidOperationException("There can only be one System Administrator.");
            }

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            return await _userRepository.AuthenticateAsync(username, password);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            return await _userRepository.ChangePasswordAsync(userId, currentPassword, newPassword);
        }

        public async Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId)
        {
            // This method would require additional repository implementation
            // For now, we'll return false
            return false;
        }

        public async Task<bool> UnassignTaskFromUserAsync(Guid taskId)
        {
            // This method would require additional repository implementation
            // For now, we'll return false
            return false;
        }

        public async Task<IEnumerable<TaskItem>> GetUserAssignedTasksAsync(Guid userId)
        {
            // This method would require additional repository implementation
            // For now, we'll return an empty list
            return Enumerable.Empty<TaskItem>();
        }

        public async Task<User> GetCurrentUserAsync(string username)
        {
            return await GetUserByUsernameAsync(username);
        }

        public async Task<bool> UpdateUserLastLoginAsync(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.LastLogin = DateTime.UtcNow;
            await UpdateUserAsync(user);
            return true;
        }

        public async Task<IEnumerable<User>> GetRecentUsersAsync(int count = 5)
        {
            var allUsers = await GetAllUsersAsync();
            return allUsers
                .OrderByDescending(u => u.CreatedAt)
                .Take(count);
        }
    }
}