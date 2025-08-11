using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Core.Models
{
    public class TaskComment
    {
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public Guid TaskId { get; set; }

        public Guid UserId { get; set; }

        // Navigation properties
        public TaskItem? Task { get; set; }

        public User? User { get; set; }
    }
}