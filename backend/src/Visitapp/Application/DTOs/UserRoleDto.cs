using System.ComponentModel.DataAnnotations;

namespace Visitapp.Application.DTOs
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? UserName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRoleCreateDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }

    public class UserRoleUpdateDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
