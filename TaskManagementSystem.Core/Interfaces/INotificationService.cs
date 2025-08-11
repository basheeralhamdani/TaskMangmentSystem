using System.Threading.Tasks;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendTaskAssignedNotificationAsync(User user, TaskItem task);
        Task SendTaskStatusChangedNotificationAsync(User user, TaskItem task);
        Task SendTaskDueSoonNotificationAsync(User user, TaskItem task);
        Task SendTaskCompletedNotificationAsync(User user, TaskItem task);
        Task SendOverdueTaskNotificationAsync(User user, TaskItem task);
        Task SendCommentAddedNotificationAsync(User recipient, TaskItem task, TaskComment comment);

        Task SendWelcomeEmailAsync(User user);
        Task SendPasswordResetEmailAsync(User user, string resetToken);
    }
}