using System.ComponentModel.DataAnnotations;

namespace Visitapp.Application.DTOs
{
    public class ChurchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? DistrictId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DistrictName { get; set; }
    }

    public class ChurchCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El distrito es requerido")]
        public int DistrictId { get; set; }
    }

    public class ChurchUpdateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public int? DistrictId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ChurchStatisticsDto
    {
        public int TotalMembers { get; set; }
        public int TotalVisits { get; set; }
        public int ActiveLeaders { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
