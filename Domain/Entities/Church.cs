using System.ComponentModel.DataAnnotations;

namespace Visitapp.Domain.Entities
{
    public class Church
    {
        public int Id { get; private set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; private set; } = string.Empty;
        
        public int DistrictId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Navigation properties
        public virtual District District { get; private set; } = null!;
        public virtual ICollection<User> Users { get; private set; } = new List<User>();

        // Private constructor for EF
        private Church() { }

        // Factory method
        public static Church Create(string name, int districtId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Church name is required", nameof(name));
            
            if (districtId <= 0)
                throw new ArgumentException("District ID must be valid", nameof(districtId));

            return new Church
            {
                Name = name.Trim(),
                DistrictId = districtId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateInfo(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Church name is required", nameof(name));

            Name = name.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeDistrict(int districtId)
        {
            if (districtId <= 0)
                throw new ArgumentException("District ID must be valid", nameof(districtId));

            DistrictId = districtId;
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