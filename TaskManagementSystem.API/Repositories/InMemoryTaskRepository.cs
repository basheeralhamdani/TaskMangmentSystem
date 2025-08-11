using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskStatus = TaskManagementSystem.Core.Models.TaskStatus;

namespace TaskManagementSystem.API.Repositories
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly ConcurrentDictionary<Guid, TaskItem> _tasks = new();
        private readonly IUserRepository _users;

        private readonly ConcurrentDictionary<Guid, List<TaskComment>> _comments = new();


        public InMemoryTaskRepository(IUserRepository users)
        {
            _users = users;

            // Seed a sample task
            var sample = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Sample Task",
                Description = "Demo in-memory task",
                Priority = PriorityLevel.Medium,
                Status = TaskStatus.Pending,
                SystemName = "Core",
                CreationDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
            };
            _tasks[sample.Id] = sample;
        }

        public Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var list = _tasks.Values
                .OrderByDescending(t => t.CreationDate)
                .Select(AttachUser)
                .AsEnumerable();
            return Task.FromResult(list);
        }

        public Task<TaskItem> GetByIdAsync(Guid id)
        {
            _tasks.TryGetValue(id, out var task);
            return Task.FromResult(AttachUser(task));
        }

        public Task<TaskItem> AddAsync(TaskItem task)
        {
            task.CreationDate = DateTime.UtcNow;
            _tasks[task.Id] = task;
            return Task.FromResult(AttachUser(task));
        }

        public Task<TaskItem> UpdateAsync(TaskItem task)
        {
            if (!_tasks.ContainsKey(task.Id))
                throw new KeyNotFoundException("Task not found");
            _tasks[task.Id] = task;
            return Task.FromResult(AttachUser(task));
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var removed = _tasks.TryRemove(id, out _);
            return Task.FromResult(removed);
        }

        public Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId)
        {
            var list = _tasks.Values
                .Where(t => t.AssignedToUserId == userId)
                .OrderByDescending(t => t.CreationDate)
                .Select(AttachUser)
                .AsEnumerable();
            return Task.FromResult(list);
        }
        public Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskStatus status)
        {
            var list = _tasks.Values
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreationDate)
                .Select(AttachUser)
                .AsEnumerable();
            return Task.FromResult(list);
        }

        public Task<IEnumerable<TaskItem>> GetByPriorityAsync(PriorityLevel priority)
        {
            var list = _tasks.Values
                .Where(t => t.Priority == priority)
                .OrderByDescending(t => t.CreationDate)
                .Select(AttachUser)
                .AsEnumerable();
            return Task.FromResult(list);
        }

        public Task<IEnumerable<TaskItem>> GetByDueDateAsync(DateTime dueDate)
        {
            var list = _tasks.Values
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Date)
                .OrderByDescending(t => t.CreationDate)
                .Select(AttachUser)
                .AsEnumerable();
            return Task.FromResult(list);
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_tasks.Count);
        }

        public Task<int> CountByStatusAsync(TaskStatus status)
        {
            var count = _tasks.Values.Count(t => t.Status == status);
            return Task.FromResult(count);
        }

        public Task<TaskComment> AddCommentAsync(TaskComment comment)
        {
            var list = _comments.GetOrAdd(comment.TaskId, _ => new List<TaskComment>());
            comment.CreatedAt = DateTime.UtcNow;
            list.Add(comment);
            return Task.FromResult(comment);
        }

        public Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(Guid taskId)
        {
            var list = _comments.GetOrAdd(taskId, _ => new List<TaskComment>());
            return Task.FromResult(list.AsEnumerable());
        }

        private TaskItem AttachUser(TaskItem task)
        {
            if (task == null) return null;
            if (task.AssignedToUserId.HasValue)
            {
                var user = _users.GetByIdAsync(task.AssignedToUserId.Value).GetAwaiter().GetResult();
                task.AssignedToUser = user;
            }
            else
            {
                task.AssignedToUser = null;
            }
            return task;
        }
    }
}
