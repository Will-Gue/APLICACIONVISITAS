using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Enums;
using Visitapp.Domain.Interfaces;
using DomainVisit = Visitapp.Domain.Entities.Visit;

namespace Visitapp.Infrastructure.Repositories
{
    public class VisitRepository : IVisitRepository
    {
        private readonly VisitAppContext _context;

        public VisitRepository(VisitAppContext context)
        {
            _context = context;
        }

        public async Task<Visit?> GetByIdAsync(int id)
        {
            return await _context.Set<DomainVisit>()
                .Include(v => v.User)
                .Include(v => v.Contact)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Visit> CreateAsync(Visit visit)
        {
            var entry = await _context.Set<DomainVisit>().AddAsync(visit);
            return entry.Entity;
        }

        public async Task UpdateAsync(Visit visit)
        {
            _context.Set<DomainVisit>().Update(visit);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var visit = await _context.Set<DomainVisit>().FindAsync(id);
            if (visit != null)
            {
                _context.Set<DomainVisit>().Remove(visit);
            }
        }

        public async Task<IEnumerable<Visit>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<DomainVisit>()
                .Include(v => v.Contact)
                .Where(v => v.UserId == userId)
                .OrderBy(v => v.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetByContactIdAsync(int contactId)
        {
            return await _context.Set<DomainVisit>()
                .Include(v => v.User)
                .Where(v => v.ContactId == contactId)
                .OrderBy(v => v.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetByStatusAsync(VisitStatus status)
        {
            return await _context.Set<DomainVisit>()
                .Include(v => v.User)
                .Include(v => v.Contact)
                .Where(v => v.Status == status)
                .OrderBy(v => v.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetScheduledForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Set<DomainVisit>()
                .Include(v => v.User)
                .Include(v => v.Contact)
                .Where(v => v.ScheduledDate >= startDate && v.ScheduledDate <= endDate)
                .OrderBy(v => v.ScheduledDate)
                .ToListAsync();
        }

        public async Task<Visit?> GetByIdAndUserIdAsync(int id, int userId)
        {
            return await _context.Set<DomainVisit>()
                .Include(v => v.Contact)
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<DomainVisit>()
                .AnyAsync(v => v.Id == id);
        }
    }
}