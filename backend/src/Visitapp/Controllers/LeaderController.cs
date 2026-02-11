using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Models;
using Visitapp.Application.DTOs.Leader;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<LeaderController> _logger;

        public LeaderController(VisitAppContext context, ILogger<LeaderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/leader/families/{leaderId}
        [HttpGet("families/{leaderId}")]
        public async Task<ActionResult<IEnumerable<LeaderFamilyDto>>> GetFamiliesByLeader(int leaderId)
        {
            try
            {
                // Seguridad: Solo el líder autenticado puede consultar sus familias
                var currentUserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || leaderId.ToString() != currentUserId)
                    return Forbid("No tiene permiso para consultar estas familias");

                // Buscar familias asignadas al líder (por ejemplo, por UserRoles o relación específica)
                var families = await _context.Users
                    .Where(u => u.UserRoles.Any(ur => ur.RoleId == /*ID_ROL_LIDER*/ 2 && ur.UserId == leaderId && ur.IsActive))
                    .Select(u => new LeaderFamilyDto
                    {
                        FamilyId = u.UserId,
                        FamilyName = u.FullName,
                        VisitsCompleted = _context.Visits.Count(v => v.UserId == u.UserId && v.Status == "Completada"),
                        VisitsPending = _context.Visits.Count(v => v.UserId == u.UserId && v.Status == "Pendiente"),
                        LastVisitDate = _context.Visits.Where(v => v.UserId == u.UserId).OrderByDescending(v => v.ScheduledDate).Select(v => v.ScheduledDate).FirstOrDefault(),
                        Notes = _context.Visits.Where(v => v.UserId == u.UserId && !string.IsNullOrEmpty(v.Notes)).Select(v => v.Notes).ToList()
                    })
                    .ToListAsync();

                return Ok(families);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener familias del líder {LeaderId}", leaderId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
            /// <summary>
            /// GET /api/leader/families/{leaderId}
            /// Solo el líder autenticado puede consultar sus familias asignadas.
            /// </summary>
            /// <remarks>
            /// Ejemplo de uso:
            ///     GET /api/leader/families/2
            /// </remarks>
            /// <response code="200">Lista de familias</response>
            /// <response code="403">No tiene permiso</response>
            /// <response code="401">No autenticado</response>
        }
    }
}
