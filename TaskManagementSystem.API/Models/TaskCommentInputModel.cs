using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.API.Models
{
    public class TaskCommentInputModel
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
    }
}

