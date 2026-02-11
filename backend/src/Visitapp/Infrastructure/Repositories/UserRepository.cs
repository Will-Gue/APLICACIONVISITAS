using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Interfaces;
using Visitapp.Domain.Specifications;

namespace Visitapp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly VisitAppContext _context;

        public UserRepository(VisitAppContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.DomainUsers
                .Include(u => u.Church)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.DomainUsers
                .Include(u => u.Church)
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
        }

        public async Task<User?> GetBySpecificationAsync(ISpecification<User> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetBySpecificationAsync(ISpecification<User> specification, bool asNoTracking = false)
        {
            var query = ApplySpecification(specification);
            if (asNoTracking)
                query = query.AsNoTracking();
            
            return await query.ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.DomainUsers
                .AnyAsync(u => u.Email == email.ToLowerInvariant());
        }

        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _context.DomainUsers
                .AnyAsync(u => u.Phone == phone);
        }

        public async Task<User> CreateAsync(User user)
        {
            var entry = await _context.DomainUsers.AddAsync(user);
            return entry.Entity;
        }

        public async Task UpdateAsync(User user)
        {
            _context.DomainUsers.Update(user);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.DomainUsers.FindAsync(id);
            if (user != null)
            {
                _context.DomainUsers.Remove(user);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.DomainUsers
                .Include(u => u.Church)
                .ToListAsync();
        }

        public async Task<User?> GetWithRolesAsync(int id)
        {
            return await _context.DomainUsers
                .Include(u => u.Church)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<int> CountAsync(ISpecification<User> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }

        private IQueryable<User> ApplySpecification(ISpecification<User> specification)
        {
            var query = _context.DomainUsers.AsQueryable();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            if (specification.IsPagingEnabled)
                query = query.Skip(specification.Skip).Take(specification.Take);

            return query;
        }
    }
}