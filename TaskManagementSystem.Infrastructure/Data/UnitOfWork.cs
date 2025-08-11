using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITaskRepository? _taskRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITaskRepository Tasks => 
            _taskRepository ??= new TaskRepository(_context);

        public IUserRepository Users => 
            _userRepository ??= new UserRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}