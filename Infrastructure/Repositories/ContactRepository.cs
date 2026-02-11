using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Interfaces;
using DomainContact = Visitapp.Domain.Entities.Contact;

namespace Visitapp.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly VisitAppContext _context;

        public ContactRepository(VisitAppContext context)
        {
            _context = context;
        }

        public async Task<Contact?> GetByIdAsync(int id)
        {
            return await _context.Set<DomainContact>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            var entry = await _context.Set<DomainContact>().AddAsync(contact);
            return entry.Entity;
        }

        public async Task UpdateAsync(Contact contact)
        {
            _context.Set<DomainContact>().Update(contact);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await _context.Set<DomainContact>().FindAsync(id);
            if (contact != null)
            {
                _context.Set<DomainContact>().Remove(contact);
            }
        }

        public async Task<IEnumerable<Contact>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<DomainContact>()
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<DomainContact>()
                .AnyAsync(c => c.Id == id);
        }

        public async Task<Contact?> GetByIdAndUserIdAsync(int id, int userId)
        {
            return await _context.Set<DomainContact>()
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }
    }
}