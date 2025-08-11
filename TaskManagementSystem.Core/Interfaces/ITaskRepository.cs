using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem> GetByIdAsync(Guid id);
        Task<TaskItem> AddAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<TaskItem>> GetByStatusAsync(Core.Models.TaskStatus status);
        Task<IEnumerable<TaskItem>> GetByPriorityAsync(PriorityLevel priority);
        Task<IEnumerable<TaskItem>> GetByDueDateAsync(DateTime dueDate);
        Task<int> CountAsync();
        Task<int> CountByStatusAsync(Core.Models.TaskStatus status);

        // Comments
        Task<TaskComment> AddCommentAsync(TaskComment comment);
        Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(Guid taskId);

    }
}