using Microsoft.AspNetCore.SignalR;
using TaskManagementSystem.API.Hubs;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendTaskAssignedNotificationAsync(User user, TaskItem task)
        {
            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("TaskAssigned", new
            {
                taskId = task.Id,
                title = task.Title,
                message = $"A new task '{task.Title}' was assigned to you",
                when = DateTime.UtcNow
            });
        }

        public async Task SendTaskStatusChangedNotificationAsync(User user, TaskItem task)
        {
            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("TaskStatusChanged", new
            {
                taskId = task.Id,
                title = task.Title,
                status = task.Status.ToString(),
                message = $"Task '{task.Title}' status changed to {task.Status}",
                when = DateTime.UtcNow
            });
        }

        public async Task SendTaskDueSoonNotificationAsync(User user, TaskItem task)
        {
            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("TaskDueSoon", new
            {
                taskId = task.Id,
                title = task.Title,
                due = task.DueDate,
                message = $"Task '{task.Title}' is due soon",
                when = DateTime.UtcNow
            });
        }

        public async Task SendTaskCompletedNotificationAsync(User user, TaskItem task)
        {
            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("TaskCompleted", new
            {
                taskId = task.Id,
                title = task.Title,
                message = $"Task '{task.Title}' was completed",
                when = DateTime.UtcNow
            });
        }

        public async Task SendOverdueTaskNotificationAsync(User user, TaskItem task)
        {
            await _hubContext.Clients.User(user.Id.ToString()).SendAsync("TaskOverdue", new
            {
                taskId = task.Id,
                title = task.Title,
                message = $"Task '{task.Title}' is overdue",
                when = DateTime.UtcNow
            });
        }

        public async Task SendCommentAddedNotificationAsync(User recipient, TaskItem task, TaskComment comment)
        {
            var payload = new
            {
                taskId = task.Id,
                title = task.Title,
                comment = comment.Content,
                authorId = comment.UserId,
                message = $"New comment on '{task.Title}': {comment.Content}",
                when = DateTime.UtcNow
            };

            await _hubContext.Clients.User(recipient.Id.ToString()).SendAsync("TaskCommentAdded", payload);
            await _hubContext.Clients.Group($"task:{task.Id}").SendAsync("TaskCommentAdded", payload);
        }


        public Task SendWelcomeEmailAsync(User user) => Task.CompletedTask;
        public Task SendPasswordResetEmailAsync(User user, string resetToken) => Task.CompletedTask;
    }
}

