using TaskManagementSystem.Core.Interfaces;

namespace TaskManagementSystem.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}