using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Core.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public PriorityLevel Priority { get; set; }

        public TaskStatus Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string SystemName { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; }

        public DateTime? DueDate { get; set; }

        public Guid? AssignedToUserId { get; set; }

        // Navigation properties
        public User? AssignedToUser { get; set; }

        public ICollection<TaskComment>? Comments { get; set; }
    }
}