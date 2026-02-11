using System.ComponentModel.DataAnnotations;
using Visitapp.Application.DTOs.Auth;

namespace Visitapp.Application.Commands.Auth
{
    public class RegisterCommand : ICommand<AuthResponse>
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone format")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        public int? ChurchId { get; set; }
    }
}