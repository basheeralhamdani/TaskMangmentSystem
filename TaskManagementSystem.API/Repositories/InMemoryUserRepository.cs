using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<Guid, User> _users = new();

        public InMemoryUserRepository()
        {
            // Seed a couple of users for demo
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "Admin123!",
                Role = UserRole.SystemAdministrator,
                CreatedAt = DateTime.UtcNow
            };
            _users[admin.Id] = admin;

            var manager = new User
            {
                Id = Guid.NewGuid(),
                Username = "manager",
                Email = "manager@example.com",
                PasswordHash = "Manager123!",
                Role = UserRole.Manager,
                CreatedAt = DateTime.UtcNow
            };
            _users[manager.Id] = manager;
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            var list = _users.Values.OrderBy(u => u.Username).AsEnumerable();
            return Task.FromResult(list);
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            _users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            var user = _users.Values.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User> AddAsync(User user)
        {
            if (_users.Values.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already exists.");
            if (_users.Values.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already exists.");

            user.CreatedAt = DateTime.UtcNow;
            _users[user.Id] = user;
            return Task.FromResult(user);
        }

        public Task<User> UpdateAsync(User user)
        {
            if (!_users.ContainsKey(user.Id))
                throw new KeyNotFoundException("User not found");

            // Enforce uniqueness on username/email across other users
            if (_users.Values.Any(u => u.Id != user.Id && u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already exists.");
            if (_users.Values.Any(u => u.Id != user.Id && u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already exists.");

            _users[user.Id] = user;
            return Task.FromResult(user);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var removed = _users.TryRemove(id, out _);
            return Task.FromResult(removed);
        }

        public Task<User> AuthenticateAsync(string username, string password)
        {
            var user = _users.Values.FirstOrDefault(u =>
                (u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) || u.Email.Equals(username, StringComparison.OrdinalIgnoreCase))
                && u.PasswordHash == password);
            return Task.FromResult(user);
        }

        public Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            if (_users.TryGetValue(userId, out var user) && user.PasswordHash == currentPassword)
            {
                user.PasswordHash = newPassword;
                _users[userId] = user;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
