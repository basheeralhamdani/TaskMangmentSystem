using System.Collections.Generic;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Models
{
    public class ReportsViewModel
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int HighPriorityTasks { get; set; }
        public Dictionary<Core.Models.TaskStatus, int> TasksByStatus { get; set; }
        public Dictionary<PriorityLevel, int> TasksByPriority { get; set; }
    }
}