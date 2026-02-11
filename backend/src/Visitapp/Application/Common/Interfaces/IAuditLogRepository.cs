using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visitapp.Domain.Entities;

namespace Visitapp.Application.Common.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<Visitapp.Domain.Models.AuditLog>> GetAllAsync();
        Task<IEnumerable<Visitapp.Domain.Models.AuditLog>> GetFilteredAsync(string userId, string module, DateTime? from, DateTime? to);
        Task<Visitapp.Domain.Models.AuditLog> AddAsync(Visitapp.Domain.Models.AuditLog log);
        Task ExportToPdfAsync(IEnumerable<Visitapp.Domain.Models.AuditLog> logs, string filePath);
    }
}