using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(Guid id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            return await _taskRepository.AddAsync(task);
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            return await _taskRepository.UpdateAsync(task);
        }

        public async Task<bool> DeleteTaskAsync(Guid id)
        {
            return await _taskRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksAssignedToUserAsync(Guid userId)
        {
            return await _taskRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(Core.Models.TaskStatus status)
        {
            return await _taskRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(PriorityLevel priority)
        {
            return await _taskRepository.GetByPriorityAsync(priority);
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            var allTasks = await GetAllTasksAsync();
            return allTasks.Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.Now && t.Status != Core.Models.TaskStatus.Completed);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksDueSoonAsync(int days = 3)
        {
            var allTasks = await GetAllTasksAsync();
            var dueDate = DateTime.Now.AddDays(days);
            return allTasks.Where(t => t.DueDate.HasValue && 
                                     t.DueDate.Value >= DateTime.Now && 
                                     t.DueDate.Value <= dueDate &&
                                     t.Status != Core.Models.TaskStatus.Completed);
        }

        public async Task<IEnumerable<TaskItem>> GetRecentTasksAsync(int count = 10)
        {
            var allTasks = await GetAllTasksAsync();
            return allTasks.Take(count);
        }

        public async Task<int> GetTaskCountAsync()
        {
            return await _taskRepository.CountAsync();
        }

        public async Task<int> GetTotalTasksAsync()
        {
            return await _taskRepository.CountAsync();
        }

        public async Task<int> GetCompletedTasksAsync()
        {
            return await _taskRepository.CountByStatusAsync(Core.Models.TaskStatus.Completed);
        }

        public async Task<int> GetPendingTasksAsync()
        {
            return await _taskRepository.CountByStatusAsync(Core.Models.TaskStatus.Pending);
        }

        public async Task<int> GetHighPriorityTasksAsync()
        {
            var allTasks = await _taskRepository.GetByPriorityAsync(PriorityLevel.High);
            // Critical may also be considered high priority
            var criticalTasks = await _taskRepository.GetByPriorityAsync(PriorityLevel.Critical);
            return allTasks.Count() + criticalTasks.Count();
        }

        public async Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId)
        {
            var task = await GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return false;
            }

            task.AssignedToUserId = userId;
            await UpdateTaskAsync(task);
            return true;
        }

        public async Task<bool> ChangeTaskStatusAsync(Guid taskId, Core.Models.TaskStatus newStatus)
        {
            var task = await GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return false;
            }

            task.Status = newStatus;
            await UpdateTaskAsync(task);
            return true;
        }

        public async Task<bool> UpdateTaskPriorityAsync(Guid taskId, PriorityLevel newPriority)
        {
            var task = await GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return false;
            }

            task.Priority = newPriority;
            await UpdateTaskAsync(task);
            return true;
        }

        public async Task<bool> UpdateTaskDueDateAsync(Guid taskId, DateTime? newDueDate)
        {
            var task = await GetTaskByIdAsync(taskId);
            if (task == null)
            {
                return false;
            }

            task.DueDate = newDueDate;
            await UpdateTaskAsync(task);
            return true;
        }

        public async Task<bool> AddCommentToTaskAsync(Guid taskId, string comment, Guid userId)
        {
            var task = await GetTaskByIdAsync(taskId);
            if (task == null || string.IsNullOrWhiteSpace(comment)) return false;

            var entity = new TaskComment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = comment.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            await _taskRepository.AddCommentAsync(entity);
            return true;
        }

        public async Task<IEnumerable<TaskComment>> GetTaskCommentsAsync(Guid taskId)
        {
            return await _taskRepository.GetCommentsByTaskIdAsync(taskId);
        }
    }
}