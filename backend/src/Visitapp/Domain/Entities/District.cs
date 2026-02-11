using System.ComponentModel.DataAnnotations;

namespace Visitapp.Domain.Entities
{
    public class District
    {
        public int Id { get; private set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; private set; } = string.Empty;
        
        [MaxLength(200)]
        public string? Description { get; private set; }
        
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Navigation properties
        public virtual ICollection<Church> Churches { get; private set; } = new List<Church>();

        // Private constructor for EF
        private District() { }

        // Factory method
        public static District Create(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("District name is required", nameof(name));

            return new District
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
                throw new ArgumentException("District name is required", nameof(name));

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