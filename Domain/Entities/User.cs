using System.ComponentModel.DataAnnotations;

namespace Visitapp.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; private set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; private set; } = string.Empty;
        
        [Required]
        [Phone]
        [MaxLength(20)]
        public string Phone { get; private set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; private set; } = string.Empty;
        
        public bool IsVerified { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        // Navigation properties
        public int? ChurchId { get; private set; }
        public virtual Church? Church { get; private set; }
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
        public virtual ICollection<Contact> Contacts { get; private set; } = new List<Contact>();
        public virtual ICollection<Visit> Visits { get; private set; } = new List<Visit>();

        // Private constructor for EF
        private User() { }

        // Factory method
        public static User Create(string fullName, string email, string phone, string passwordHash, int? churchId = null)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required", nameof(fullName));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));
            
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone is required", nameof(phone));
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required", nameof(passwordHash));

            return new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                Phone = phone.Trim(),
                PasswordHash = passwordHash,
                ChurchId = churchId,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateProfile(string fullName, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required", nameof(fullName));
            
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone is required", nameof(phone));

            FullName = fullName.Trim();
            Phone = phone.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash is required", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Verify()
        {
            IsVerified = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignToChurch(int churchId)
        {
            ChurchId = churchId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}