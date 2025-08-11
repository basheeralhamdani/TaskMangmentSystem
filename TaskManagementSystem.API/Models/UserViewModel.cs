using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using TaskManagementSystem.Core.Models;

namespace TaskManagementSystem.API.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [ValidateNever]
        public string PasswordHash { get; set; } = string.Empty; // only for create form binding
        [Required]
        public UserRole Role { get; set; }
        [BindNever, ValidateNever]
        public DateTime CreatedAt { get; set; }

        [BindNever, ValidateNever]
        public DateTime? LastLogin { get; set; }

        [ValidateNever]
        public int AssignedTasksCount { get; set; }
    }
}