using System;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Services
{
    public class EmailNotificationService : INotificationService
    {
        public async Task SendTaskAssignedNotificationAsync(User user, TaskItem task)
        {
            // In a real implementation, this would send an email
            // For now, we'll just log the action
            Console.WriteLine($"Email: Task '{task.Title}' assigned to {user.Username}.");
            await Task.CompletedTask;
        }

        public async Task SendTaskStatusChangedNotificationAsync(User user, TaskItem task)
        {
            // In a real implementation, this would send an email
            Console.WriteLine($"Email: Task '{task.Title}' status changed to {task.Status} by {user.Username}.");
            await Task.CompletedTask;
        }

        public async Task SendTaskDueSoonNotificationAsync(User user, TaskItem task)
        {
            // In a real implementation, this would send an email
            Console.WriteLine($"Email: Task '{task.Title}' is due soon for {user.Username}.");
            await Task.CompletedTask;
        }

        public async Task SendTaskCompletedNotificationAsync(User user, TaskItem task)
        {
            // In a real implementation, this would send an email
            Console.WriteLine($"Email: Task '{task.Title}' completed by {user.Username}.");
            await Task.CompletedTask;
        }

        public async Task SendOverdueTaskNotificationAsync(User user, TaskItem task)
        {
            // In a real implementation, this would send an email
            Console.WriteLine($"Email: Task '{task.Title}' is overdue for {user.Username}.");
            await Task.CompletedTask;
        }

        public async Task SendWelcomeEmailAsync(User user)
        {
            // In a real implementation, this would send an email
            Console.WriteLine($"Email: Welcome {user.Username} to Task Management System.");
            await Task.CompletedTask;
        }

        public async Task SendPasswordResetEmailAsync(User user, string resetToken)
        {
            // In a real implementation, this would send an email
            Console.WriteLine($"Email: Password reset link sent to {user.Username}.");
            await Task.CompletedTask;
        }

		public async Task SendCommentAddedNotificationAsync(User recipient, TaskItem task, TaskComment comment)
		{
		    Console.WriteLine($"Email: New comment on '{task.Title}' for {recipient.Username}: {comment.Content}");
		    await Task.CompletedTask;
		}

    }
}