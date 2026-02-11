namespace Visitapp.Domain.Entities
{
    public class UserRole
    {
        public int UserId { get; private set; }
        public int RoleId { get; private set; }
        public DateTime AssignedDate { get; private set; }
        public DateTime? RevokedDate { get; private set; }
        public bool IsActive { get; private set; }
        
        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual Role Role { get; private set; } = null!;

        // Private constructor for EF
        private UserRole() { }

        // Factory method
        public static UserRole Create(int userId, int roleId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be valid", nameof(userId));
            
            if (roleId <= 0)
                throw new ArgumentException("Role ID must be valid", nameof(roleId));

            return new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedDate = DateTime.UtcNow,
                IsActive = true
            };
        }

        public void Revoke()
        {
            IsActive = false;
            RevokedDate = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            IsActive = true;
            RevokedDate = null;
        }
    }
}