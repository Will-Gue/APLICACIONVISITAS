using Microsoft.EntityFrameworkCore.Storage;
using Visitapp.Data;
using Visitapp.Domain.Interfaces;

namespace Visitapp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VisitAppContext _context;
        private IDbContextTransaction? _transaction;
        
        private IUserRepository? _users;
        private IContactRepository? _contacts;
        private IVisitRepository? _visits;

        public UnitOfWork(VisitAppContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IContactRepository Contacts => _contacts ??= new ContactRepository(_context);
        public IVisitRepository Visits => _visits ??= new VisitRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}