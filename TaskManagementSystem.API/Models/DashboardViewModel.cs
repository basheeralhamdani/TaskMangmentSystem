using System;
using System.Collections.Generic;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Models
{
    public class DashboardViewModel
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int HighPriorityTasks { get; set; }
        public IEnumerable<TaskItem> RecentTasks { get; set; } = Array.Empty<TaskItem>();
    }
}
