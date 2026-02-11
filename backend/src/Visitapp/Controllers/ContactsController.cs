using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Models;
using Visitapp.Dtos;
using Visitapp.Application.Common.Interfaces;
using Visitapp.Domain.Models;
using System.Security.Claims;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<ContactsController> _logger;
        private readonly IAuditLogRepository _auditLogRepository;

        public ContactsController(VisitAppContext context, ILogger<ContactsController> logger, IAuditLogRepository auditLogRepository)
        {
            _context = context;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            try
            {
                var contacts = await _context.Contacts
                    .Include(c => c.User)
                    .Select(c => new ContactDto
                    {
                        ContactId = c.ContactId,
                        UserId = c.UserId,
                        FullName = c.FullName,
                        Phone = c.Phone,
                        Email = c.Email,
                        Category = c.Category,
                        CreatedAt = c.CreatedAt,
                        UserName = c.User != null ? c.User.FullName : null
                    })
                    .ToListAsync();

                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "GET_ALL",
                    Module = "Contacts",
                    Date = DateTime.UtcNow,
                    Details = $"Consulta de todos los contactos ({contacts.Count} registros)"
                });
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contactos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContact(int id)
        {
            try
            {
                var contact = await _context.Contacts
                    .Include(c => c.User)
                    .Where(c => c.ContactId == id)
                    .Select(c => new ContactDto
                    {
                        ContactId = c.ContactId,
                        UserId = c.UserId,
                        FullName = c.FullName,
                        Phone = c.Phone,
                        Email = c.Email,
                        Category = c.Category,
                        CreatedAt = c.CreatedAt,
                        UserName = c.User != null ? c.User.FullName : null
                    })
                    .FirstOrDefaultAsync();

                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "GET_BY_ID",
                    Module = "Contacts",
                    Date = DateTime.UtcNow,
                    Details = $"Consulta de contacto id={id}"
                });
                return Ok(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contacto {ContactId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> PostContact(ContactCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creando contacto para usuario {UserId}", dto.UserId);

                if (string.IsNullOrWhiteSpace(dto.FullName))
                    return BadRequest(new { message = "El nombre completo es requerido" });

                var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
                if (!userExists)
                    return BadRequest(new { message = "El usuario especificado no existe" });

                var contact = new Contacts
                {
                    UserId = dto.UserId,
                    FullName = dto.FullName.Trim(),
                    Phone = dto.Phone?.Trim(),
                    Email = dto.Email?.Trim(),
                    Category = dto.Category?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                var createdContact = await _context.Contacts
                    .Include(c => c.User)
                    .Where(c => c.ContactId == contact.ContactId)
                    .Select(c => new ContactDto
                    {
                        ContactId = c.ContactId,
                        UserId = c.UserId,
                        FullName = c.FullName,
                        Phone = c.Phone,
                        Email = c.Email,
                        Category = c.Category,
                        CreatedAt = c.CreatedAt,
                        UserName = c.User != null ? c.User.FullName : null
                    })
                    .FirstAsync();

                _logger.LogInformation("Contacto {ContactId} creado exitosamente", contact.ContactId);
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "CREATE",
                    Module = "Contacts",
                    Date = DateTime.UtcNow,
                    Details = $"Contacto creado: id={contact.ContactId}, usuario={contact.UserId}"
                });
                return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, createdContact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear contacto");
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, ContactUpdateDto dto)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                if (string.IsNullOrWhiteSpace(dto.FullName))
                    return BadRequest(new { message = "El nombre completo es requerido" });

                contact.FullName = dto.FullName.Trim();
                contact.Phone = dto.Phone?.Trim();
                contact.Email = dto.Email?.Trim();
                contact.Category = dto.Category?.Trim();

                await _context.SaveChangesAsync();
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "UPDATE",
                    Module = "Contacts",
                    Date = DateTime.UtcNow,
                    Details = $"Contacto actualizado: id={id}"
                });
                _logger.LogInformation("Contacto {ContactId} actualizado", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar contacto {ContactId}", id);
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando contacto {ContactId}", id);

                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                var relatedVisits = await _context.Visits
                    .Where(v => v.ContactId == id)
                    .ToListAsync();

                if (relatedVisits.Any())
                {
                    _logger.LogInformation("Eliminando {Count} visitas relacionadas", relatedVisits.Count);
                    _context.Visits.RemoveRange(relatedVisits);
                }

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "DELETE",
                    Module = "Contacts",
                    Date = DateTime.UtcNow,
                    Details = $"Contacto eliminado: id={id}"
                });
                _logger.LogInformation("Contacto {ContactId} eliminado exitosamente", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar contacto {ContactId}", id);
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContactsByUser(int userId)
        {
            try
            {
                var contacts = await _context.Contacts
                    .Include(c => c.User)
                    .Where(c => c.UserId == userId)
                    .Select(c => new ContactDto
                    {
                        ContactId = c.ContactId,
                        UserId = c.UserId,
                        FullName = c.FullName,
                        Phone = c.Phone,
                        Email = c.Email,
                        Category = c.Category,
                        CreatedAt = c.CreatedAt,
                        UserName = c.User != null ? c.User.FullName : null
                    })
                    .ToListAsync();

                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "GET_BY_USER",
                    Module = "Contacts",
                    Date = DateTime.UtcNow,
                    Details = $"Consulta de contactos por usuario: userId={userId}, total={contacts.Count}"
                });
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contactos del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}