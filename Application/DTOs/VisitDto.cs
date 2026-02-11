using System.ComponentModel.DataAnnotations;

namespace Visitapp.Application.DTOs
{
    public class VisitDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ContactName { get; set; }
    }

    public class VisitCreateDto
    {
        [Required(ErrorMessage = "El contacto es requerido")]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime ScheduledDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class VisitUpdateDto
    {
        [Required]
        public DateTime ScheduledDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
    }
}
