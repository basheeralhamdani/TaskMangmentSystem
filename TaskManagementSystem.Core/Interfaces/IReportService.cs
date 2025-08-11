using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.Core.Interfaces
{
    public interface IReportService
    {
        Task<Dictionary<Core.Models.TaskStatus, int>> GetTaskStatusReportAsync();
        Task<Dictionary<PriorityLevel, int>> GetTaskPriorityReportAsync();
        Task<Dictionary<User, int>> GetUserTaskReportAsync();
        Task<Dictionary<string, int>> GetMonthlyTaskReportAsync(int year);
        Task<Dictionary<string, int>> GetWeeklyTaskReportAsync();
        Task<Dictionary<string, int>> GetDailyTaskReportAsync();
        Task<Dictionary<User, int>> GetUserPerformanceReportAsync();
        Task<Dictionary<string, int>> GetTaskCompletionTimeReportAsync();
    }
}