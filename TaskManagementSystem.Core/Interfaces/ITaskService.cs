using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(Guid id);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetTasksAssignedToUserAsync(Guid userId);
        Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Core.Models.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(PriorityLevel priority);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksDueSoonAsync(int days = 3);
        Task<IEnumerable<TaskItem>> GetRecentTasksAsync(int count = 10);
        Task<int> GetTaskCountAsync();
        Task<int> GetTotalTasksAsync();
        Task<int> GetCompletedTasksAsync();
        Task<int> GetPendingTasksAsync();
        Task<int> GetHighPriorityTasksAsync();
        Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId);
        Task<bool> ChangeTaskStatusAsync(Guid taskId, Core.Models.TaskStatus newStatus);
        Task<bool> UpdateTaskPriorityAsync(Guid taskId, PriorityLevel newPriority);
        Task<bool> UpdateTaskDueDateAsync(Guid taskId, DateTime? newDueDate);
        Task<bool> AddCommentToTaskAsync(Guid taskId, string comment, Guid userId);
        Task<IEnumerable<TaskComment>> GetTaskCommentsAsync(Guid taskId);
    }
}