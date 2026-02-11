using Visitapp.Domain.Entities;
using Visitapp.Domain.Enums;

namespace Visitapp.Domain.Interfaces
{
    public interface IVisitRepository
    {
        Task<Visit?> GetByIdAsync(int id);
        Task<Visit> CreateAsync(Visit visit);
        Task UpdateAsync(Visit visit);
        Task DeleteAsync(int id);
        Task<IEnumerable<Visit>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Visit>> GetByContactIdAsync(int contactId);
        Task<IEnumerable<Visit>> GetByStatusAsync(VisitStatus status);
        Task<IEnumerable<Visit>> GetScheduledForDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Visit?> GetByIdAndUserIdAsync(int id, int userId);
        Task<bool> ExistsAsync(int id);
    }
}