using System;

namespace Visitapp.Domain.Entities
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
}