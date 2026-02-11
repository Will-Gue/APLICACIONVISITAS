using Visitapp.Domain.Entities;

namespace Visitapp.Models
{
    /// <summary>
    /// Legacy alias para mantener compatibilidad con controladores v1
    /// Apunta directamente a User del dominio
    /// </summary>
    public class Users : User
    {
        // Propiedades adicionales de la tabla legacy
        public int UserId { get; set; }  // ID legacy
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? ChurchId { get; set; }
    }
}
