using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId);
        Task<bool> UnassignTaskFromUserAsync(Guid taskId);
        Task<IEnumerable<TaskItem>> GetUserAssignedTasksAsync(Guid userId);
    }
}