using System;
using System.Collections.Generic;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Models
{
    public class TaskStatisticsViewModel
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int HighPriorityTasks { get; set; }
        public Dictionary<Core.Models.TaskStatus, int> TasksByStatus { get; set; }
        public Dictionary<PriorityLevel, int> TasksByPriority { get; set; }
        public Dictionary<string, int> TasksBySystem { get; set; }
        public Dictionary<DateTime, int> TasksByDate { get; set; }
    }
}