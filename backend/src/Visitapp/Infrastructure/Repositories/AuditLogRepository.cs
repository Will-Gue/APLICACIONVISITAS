using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Visitapp.Domain.Entities;
using Visitapp.Application.Common.Interfaces;

namespace Visitapp.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly Visitapp.Data.VisitAppContext _context;

        public AuditLogRepository(Visitapp.Data.VisitAppContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await Task.FromResult(_context.Set<AuditLog>().AsQueryable().ToList());
        }

        public async Task<IEnumerable<AuditLog>> GetFilteredAsync(string userId, string module, DateTime? from, DateTime? to)
        {
            var query = _context.Set<AuditLog>().AsQueryable();
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(l => l.UserId == userId);
            if (!string.IsNullOrEmpty(module))
                query = query.Where(l => l.Module == module);
            if (from.HasValue)
                query = query.Where(l => l.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.Timestamp <= to.Value);
            return await Task.FromResult(query.ToList());
        }

        public async Task<AuditLog> AddAsync(AuditLog log)
        {
            _context.Set<AuditLog>().Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public Task ExportToPdfAsync(IEnumerable<AuditLog> logs, string filePath)
        {
            // Exportar a CSV
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                writer.WriteLine("AuditLogId,UserId,UserName,Action,Module,Timestamp,Details");
                foreach (var log in logs)
                {
                    writer.WriteLine($"{log.AuditLogId},{log.UserId},{log.UserName},{log.Action},{log.Module},{log.Timestamp:O},{log.Details?.Replace(","," ")}");
                }
            }
            return Task.CompletedTask;
        }
    }
}