using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<VisitsController> _logger;

        public VisitsController(VisitAppContext context, ILogger<VisitsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitDto>>> GetVisits()
        {
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Select(v => new VisitDto
                    {
                        VisitId = v.VisitId,
                        UserId = v.UserId,
                        ContactId = v.ContactId,
                        ScheduledDate = v.ScheduledDate,
                        Status = v.Status,
                        Notes = v.Notes,
                        CreatedAt = v.CreatedAt,
                        UserName = v.User != null ? v.User.FullName : null,
                        ContactName = v.Contact != null ? v.Contact.FullName : null,
                        ContactPhone = v.Contact != null ? v.Contact.Phone : null,
                        ContactCategory = v.Contact != null ? v.Contact.Category : null
                    })
                    .OrderByDescending(v => v.ScheduledDate)
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VisitDto>> GetVisit(int id)
        {
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.VisitId == id)
                    .Select(v => new VisitDto
                    {
                        VisitId = v.VisitId,
                        UserId = v.UserId,
                        ContactId = v.ContactId,
                        ScheduledDate = v.ScheduledDate,
                        Status = v.Status,
                        Notes = v.Notes,
                        CreatedAt = v.CreatedAt,
                        UserName = v.User != null ? v.User.FullName : null,
                        ContactName = v.Contact != null ? v.Contact.FullName : null,
                        ContactPhone = v.Contact != null ? v.Contact.Phone : null,
                        ContactCategory = v.Contact != null ? v.Contact.Category : null
                    })
                    .FirstOrDefaultAsync();

                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                return Ok(visit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<VisitDto>> PostVisit(VisitCreateDto dto)
        {
            try
            {
                // Verificar que el usuario existe
                var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
                if (!userExists)
                    return BadRequest(new { message = "El usuario especificado no existe" });

                // Verificar que el contacto existe
                var contactExists = await _context.Contacts.AnyAsync(c => c.ContactId == dto.ContactId);
                if (!contactExists)
                    return BadRequest(new { message = "El contacto especificado no existe" });

                // Validar fecha
                if (dto.ScheduledDate < DateTime.Now.AddMinutes(-5))
                    return BadRequest(new { message = "La fecha programada no puede ser en el pasado" });

                var visit = new Visits
                {
                    UserId = dto.UserId,
                    ContactId = dto.ContactId,
                    ScheduledDate = dto.ScheduledDate,
                    Status = dto.Status,
                    Notes = dto.Notes?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();

                // Obtener la visita creada con información relacionada
                var createdVisit = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.VisitId == visit.VisitId)
                    .Select(v => new VisitDto
                    {
                        VisitId = v.VisitId,
                        UserId = v.UserId,
                        ContactId = v.ContactId,
                        ScheduledDate = v.ScheduledDate,
                        Status = v.Status,
                        Notes = v.Notes,
                        CreatedAt = v.CreatedAt,
                        UserName = v.User != null ? v.User.FullName : null,
                        ContactName = v.Contact != null ? v.Contact.FullName : null,
                        ContactPhone = v.Contact != null ? v.Contact.Phone : null,
                        ContactCategory = v.Contact != null ? v.Contact.Category : null
                    })
                    .FirstAsync();

                return CreatedAtAction(nameof(GetVisit), new { id = visit.VisitId }, createdVisit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear visita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisit(int id, VisitUpdateDto dto)
        {
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                // Seguridad: Solo el usuario propietario puede editar
                var currentUserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || visit.UserId.ToString() != currentUserId)
                    return Forbid("No tiene permiso para editar esta visita");

                // Validar fecha
                if (dto.ScheduledDate < DateTime.Now.AddMinutes(-5))
                    return BadRequest(new { message = "La fecha programada no puede ser en el pasado" });

                visit.ScheduledDate = dto.ScheduledDate;
                visit.Status = dto.Status;
                visit.Notes = dto.Notes?.Trim();

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            try
            {
                var visit = await _context.Visits.FindAsync(id);
                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                // Seguridad: Solo el usuario propietario puede eliminar
                var currentUserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || visit.UserId.ToString() != currentUserId)
                    return Forbid("No tiene permiso para eliminar esta visita");

                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
            /// <summary>
            /// PUT /api/visits/{id}
            /// Solo el usuario propietario puede editar su visita.
            /// </summary>
            /// <remarks>
            /// Ejemplo de request:
            ///     PUT /api/visits/1
            ///     {
            ///         "scheduledDate": "2026-02-10T10:00:00Z",
            ///         "status": "Completada",
            ///         "notes": "Visita realizada con éxito"
            ///     }
            /// </remarks>
            /// <response code="204">Actualización exitosa</response>
            /// <response code="403">No tiene permiso</response>
            /// <response code="404">No encontrada</response>
            /// <response code="400">Datos inválidos</response>

        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<VisitDto>>> GetVisitsByUser(int userId)
        {
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.UserId == userId)
                    .Select(v => new VisitDto
                    {
                        VisitId = v.VisitId,
                        UserId = v.UserId,
                        ContactId = v.ContactId,
                        ScheduledDate = v.ScheduledDate,
                        Status = v.Status,
                        Notes = v.Notes,
                        CreatedAt = v.CreatedAt,
                        UserName = v.User != null ? v.User.FullName : null,
                        ContactName = v.Contact != null ? v.Contact.FullName : null,
                        ContactPhone = v.Contact != null ? v.Contact.Phone : null,
                        ContactCategory = v.Contact != null ? v.Contact.Category : null
                    })
                    .OrderByDescending(v => v.ScheduledDate)
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("contact/{contactId}")]
        public async Task<ActionResult<IEnumerable<VisitDto>>> GetVisitsByContact(int contactId)
        {
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.ContactId == contactId)
                    .Select(v => new VisitDto
                    {
                        VisitId = v.VisitId,
                        UserId = v.UserId,
                        ContactId = v.ContactId,
                        ScheduledDate = v.ScheduledDate,
                        Status = v.Status,
                        Notes = v.Notes,
                        CreatedAt = v.CreatedAt,
                        UserName = v.User != null ? v.User.FullName : null,
                        ContactName = v.Contact != null ? v.Contact.FullName : null,
                        ContactPhone = v.Contact != null ? v.Contact.Phone : null,
                        ContactCategory = v.Contact != null ? v.Contact.Category : null
                    })
                    .OrderByDescending(v => v.ScheduledDate)
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas del contacto {ContactId}", contactId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<VisitDto>>> GetVisitsByStatus(string status)
        {
            try
            {
                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.Status == status)
                    .Select(v => new VisitDto
                    {
                        VisitId = v.VisitId,
                        UserId = v.UserId,
                        ContactId = v.ContactId,
                        ScheduledDate = v.ScheduledDate,
                        Status = v.Status,
                        Notes = v.Notes,
                        CreatedAt = v.CreatedAt,
                        UserName = v.User != null ? v.User.FullName : null,
                        ContactName = v.Contact != null ? v.Contact.FullName : null,
                        ContactPhone = v.Contact != null ? v.Contact.Phone : null,
                        ContactCategory = v.Contact != null ? v.Contact.Category : null
                    })
                    .OrderByDescending(v => v.ScheduledDate)
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas con estatus {Status}", status);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}