using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visitapp.Application.Common.Interfaces;
using Visitapp.Domain.Entities;

namespace Visitapp.Controllers
{
    [ApiController]
    [Route("api/auditlogs")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogController(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAll()
        {
            var logs = await _repo.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetFiltered([FromQuery] string userId, [FromQuery] string module, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var logs = await _repo.GetFilteredAsync(userId, module, from, to);
            return Ok(logs);
        }

        [HttpPost]
        public async Task<ActionResult<AuditLog>> Add([FromBody] AuditLog log)
        {
            var result = await _repo.AddAsync(log);
            return CreatedAtAction(nameof(GetAll), new { id = result.AuditLogId }, result);
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] List<AuditLog> logs, [FromQuery] string filePath)
        {
            await _repo.ExportToPdfAsync(logs, filePath);
            return Ok(new { message = "CSV export successful", filePath });
        }
    }
}