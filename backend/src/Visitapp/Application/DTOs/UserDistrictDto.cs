using System.ComponentModel.DataAnnotations;

namespace Visitapp.Application.DTOs
{
    public class UserDistrictDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? UserName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDistrictCreateDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DistrictId { get; set; }
    }

    public class UserDistrictUpdateDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int DistrictId { get; set; }
        public bool IsActive { get; set; }
    }
}
