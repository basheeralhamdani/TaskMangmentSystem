using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Models
{
    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PriorityLevel Priority { get; set; }
        public Core.Models.TaskStatus Status { get; set; }
        public string SystemName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? AssignedToUserId { get; set; }

        [ValidateNever]
        public string? AssignedToUsername { get; set; }

        [ValidateNever]
        public List<TaskComment> Comments { get; set; } = new();

        public bool CreateAnother { get; set; }
    }
}