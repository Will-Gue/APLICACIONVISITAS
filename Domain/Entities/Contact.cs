using System.ComponentModel.DataAnnotations;

namespace Visitapp.Domain.Entities
{
    public class Contact
    {
        public int Id { get; private set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; private set; } = string.Empty;
        
        [Phone]
        [MaxLength(20)]
        public string? Phone { get; private set; }
        
        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; private set; }
        
        [MaxLength(50)]
        public string? Category { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Foreign keys
        public int UserId { get; private set; }
        
        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual ICollection<Visit> Visits { get; private set; } = new List<Visit>();

        // Private constructor for EF
        private Contact() { }

        // Factory method
        public static Contact Create(int userId, string fullName, string? phone = null, string? email = null, string? category = null)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be valid", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required", nameof(fullName));

            return new Contact
            {
                UserId = userId,
                FullName = fullName.Trim(),
                Phone = phone?.Trim(),
                Email = email?.Trim().ToLowerInvariant(),
                Category = category?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateInfo(string fullName, string? phone = null, string? email = null, string? category = null)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required", nameof(fullName));

            FullName = fullName.Trim();
            Phone = phone?.Trim();
            Email = email?.Trim().ToLowerInvariant();
            Category = category?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}