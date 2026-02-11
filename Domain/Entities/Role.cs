using System.ComponentModel.DataAnnotations;

namespace Visitapp.Domain.Entities
{
    public class Role
    {
        public int Id { get; private set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; private set; } = string.Empty;
        
        [MaxLength(200)]
        public string? Description { get; private set; }
        
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        // Private constructor for EF
        private Role() { }

        // Factory method
        public static Role Create(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name is required", nameof(name));

            return new Role
            {
                Name = name.Trim(),
                Description = description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateInfo(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name is required", nameof(name));

            Name = name.Trim();
            Description = description?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}