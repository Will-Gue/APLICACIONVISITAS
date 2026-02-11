namespace Visitapp.Domain.Events
{
    public class UserRegisteredEvent : IDomainEvent
    {
        public UserRegisteredEvent(int userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            OccurredOn = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }

        public int UserId { get; }
        public string Email { get; }
        public string FullName { get; }
        public DateTime OccurredOn { get; }
        public Guid EventId { get; }
    }
}