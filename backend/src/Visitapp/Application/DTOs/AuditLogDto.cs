namespace Visitapp.Application.DTOs
{
    public class AuditLogDto
    {
        public int AuditId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public string Date { get; set; }
        public string Details { get; set; }
    }
}