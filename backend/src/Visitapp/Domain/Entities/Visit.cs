using System.ComponentModel.DataAnnotations;
using Visitapp.Domain.Enums;

namespace Visitapp.Domain.Entities
{
    public class Visit
    {
        public int Id { get; private set; }
        
        public DateTime ScheduledDate { get; private set; }
        public VisitStatus Status { get; private set; }
        
        [MaxLength(500)]
        public string? Notes { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        
        // Foreign keys
        public int UserId { get; private set; }
        public int ContactId { get; private set; }
        
        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual Contact Contact { get; private set; } = null!;

        // Private constructor for EF
        private Visit() { }

        // Factory method
        public static Visit Schedule(int userId, int contactId, DateTime scheduledDate, string? notes = null)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be valid", nameof(userId));
            
            if (contactId <= 0)
                throw new ArgumentException("Contact ID must be valid", nameof(contactId));
            
            if (scheduledDate <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled date must be in the future", nameof(scheduledDate));

            return new Visit
            {
                UserId = userId,
                ContactId = contactId,
                ScheduledDate = scheduledDate,
                Notes = notes?.Trim(),
                Status = VisitStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Reschedule(DateTime newScheduledDate)
        {
            if (newScheduledDate <= DateTime.UtcNow)
                throw new ArgumentException("New scheduled date must be in the future", nameof(newScheduledDate));
            
            if (Status == VisitStatus.Completed)
                throw new InvalidOperationException("Cannot reschedule a completed visit");

            ScheduledDate = newScheduledDate;
            Status = VisitStatus.Rescheduled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel(string? reason = null)
        {
            if (Status == VisitStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed visit");

            Status = VisitStatus.Cancelled;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                Notes = string.IsNullOrWhiteSpace(Notes) 
                    ? $"Cancelled: {reason.Trim()}" 
                    : $"{Notes}\nCancelled: {reason.Trim()}";
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete(string? completionNotes = null)
        {
            if (Status == VisitStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled visit");

            Status = VisitStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(completionNotes))
            {
                Notes = string.IsNullOrWhiteSpace(Notes) 
                    ? completionNotes.Trim() 
                    : $"{Notes}\nCompleted: {completionNotes.Trim()}";
            }
            
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}