using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Models;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<TaskItem> GetByIdAsync(Guid id)
        {
            return await _context.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            task.CreationDate = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Tasks
                .Where(t => t.AssignedToUserId == userId)
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(Core.Models.TaskStatus status)
        {
            return await _context.Tasks
                .Where(t => t.Status == status)
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByPriorityAsync(PriorityLevel priority)
        {
            return await _context.Tasks
                .Where(t => t.Priority == priority)
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByDueDateAsync(DateTime dueDate)
        {
            return await _context.Tasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Date)
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Tasks.CountAsync();
        }

        public async Task<int> CountByStatusAsync(Core.Models.TaskStatus status)
        {
            return await _context.Tasks.CountAsync(t => t.Status == status);
        }

        public async Task<TaskComment> AddCommentAsync(TaskComment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(Guid taskId)
        {
            return await _context.TaskComments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

    }
}