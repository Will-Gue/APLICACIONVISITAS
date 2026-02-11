# Controlador de Visitas para el backend C# .NET

$visitsController = @'
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisitApp.Data;
using VisitApp.Models;

namespace VisitApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisitsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VisitsController> _logger;

        public VisitsController(AppDbContext context, ILogger<VisitsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/visits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVisits()
        {
            try
            {
                var userId = GetCurrentUserId();
                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.UserId == userId)
                    .OrderByDescending(v => v.ScheduledDate)
                    .Select(v => new
                    {
                        v.VisitId,
                        v.UserId,
                        v.ContactId,
                        v.ScheduledDate,
                        v.Address,
                        v.Notes,
                        v.Status,
                        v.CompletedAt,
                        v.Tema,
                        v.CreatedAt,
                        UserName = v.User.FullName,
                        ContactName = v.Contact.FullName,
                        ContactPhone = v.Contact.Phone,
                        ContactCategory = v.Contact.Category
                    })
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/visits/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVisit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var visit = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .FirstOrDefaultAsync(v => v.VisitId == id && v.UserId == userId);

                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                var result = new
                {
                    visit.VisitId,
                    visit.UserId,
                    visit.ContactId,
                    visit.ScheduledDate,
                    visit.Address,
                    visit.Notes,
                    visit.Status,
                    visit.CompletedAt,
                    visit.Tema,
                    visit.CreatedAt,
                    UserName = visit.User.FullName,
                    ContactName = visit.Contact.FullName,
                    ContactPhone = visit.Contact.Phone,
                    ContactCategory = visit.Contact.Category
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/visits
        [HttpPost]
        public async Task<ActionResult<object>> CreateVisit(VisitCreateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Verificar que el contacto existe y pertenece al usuario
                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.ContactId == dto.ContactId && c.UserId == userId);
                
                if (contact == null)
                    return BadRequest(new { message = "Contacto no encontrado" });

                var visit = new Visit
                {
                    UserId = userId,
                    ContactId = dto.ContactId,
                    ScheduledDate = dto.ScheduledDate,
                    Address = dto.Address,
                    Notes = dto.Notes,
                    Status = dto.Status ?? "Programada",
                    Tema = dto.Tema,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Visits.Add(visit);
                await _context.SaveChangesAsync();

                // Recargar con datos de navegación
                await _context.Entry(visit).Reference(v => v.User).LoadAsync();
                await _context.Entry(visit).Reference(v => v.Contact).LoadAsync();

                var result = new
                {
                    visit.VisitId,
                    visit.UserId,
                    visit.ContactId,
                    visit.ScheduledDate,
                    visit.Address,
                    visit.Notes,
                    visit.Status,
                    visit.CompletedAt,
                    visit.Tema,
                    visit.CreatedAt,
                    UserName = visit.User.FullName,
                    ContactName = visit.Contact.FullName,
                    ContactPhone = visit.Contact.Phone,
                    ContactCategory = visit.Contact.Category
                };

                return CreatedAtAction(nameof(GetVisit), new { id = visit.VisitId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear visita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/visits/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateVisit(int id, VisitUpdateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var visit = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .FirstOrDefaultAsync(v => v.VisitId == id && v.UserId == userId);

                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                // Si se cambia el contacto, verificar que existe y pertenece al usuario
                if (dto.ContactId != visit.ContactId)
                {
                    var contact = await _context.Contacts
                        .FirstOrDefaultAsync(c => c.ContactId == dto.ContactId && c.UserId == userId);
                    
                    if (contact == null)
                        return BadRequest(new { message = "Contacto no encontrado" });
                }

                visit.ContactId = dto.ContactId;
                visit.ScheduledDate = dto.ScheduledDate;
                visit.Address = dto.Address;
                visit.Notes = dto.Notes;
                visit.Status = dto.Status ?? visit.Status;
                visit.Tema = dto.Tema;

                await _context.SaveChangesAsync();

                var result = new
                {
                    visit.VisitId,
                    visit.UserId,
                    visit.ContactId,
                    visit.ScheduledDate,
                    visit.Address,
                    visit.Notes,
                    visit.Status,
                    visit.CompletedAt,
                    visit.Tema,
                    visit.CreatedAt,
                    UserName = visit.User.FullName,
                    ContactName = visit.Contact.FullName,
                    ContactPhone = visit.Contact.Phone,
                    ContactCategory = visit.Contact.Category
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/visits/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var visit = await _context.Visits
                    .FirstOrDefaultAsync(v => v.VisitId == id && v.UserId == userId);

                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Visita eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/visits/status/{status}
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<object>>> GetVisitsByStatus(string status)
        {
            try
            {
                var userId = GetCurrentUserId();
                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.UserId == userId && v.Status == status)
                    .OrderByDescending(v => v.ScheduledDate)
                    .Select(v => new
                    {
                        v.VisitId,
                        v.UserId,
                        v.ContactId,
                        v.ScheduledDate,
                        v.Address,
                        v.Notes,
                        v.Status,
                        v.CompletedAt,
                        v.Tema,
                        v.CreatedAt,
                        UserName = v.User.FullName,
                        ContactName = v.Contact.FullName,
                        ContactPhone = v.Contact.Phone,
                        ContactCategory = v.Contact.Category
                    })
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas por estado");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/visits/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<ActionResult<object>> CompleteVisit(int id, CompleteVisitDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var visit = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .FirstOrDefaultAsync(v => v.VisitId == id && v.UserId == userId);

                if (visit == null)
                    return NotFound(new { message = "Visita no encontrada" });

                visit.Status = "Completada";
                visit.CompletedAt = DateTime.UtcNow;
                visit.Notes = dto.Notes ?? visit.Notes;

                await _context.SaveChangesAsync();

                var result = new
                {
                    visit.VisitId,
                    visit.UserId,
                    visit.ContactId,
                    visit.ScheduledDate,
                    visit.Address,
                    visit.Notes,
                    visit.Status,
                    visit.CompletedAt,
                    visit.Tema,
                    visit.CreatedAt,
                    UserName = visit.User.FullName,
                    ContactName = visit.Contact.FullName,
                    ContactPhone = visit.Contact.Phone,
                    ContactCategory = visit.Contact.Category
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar visita {VisitId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/visits/contact/{contactId}
        [HttpGet("contact/{contactId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetVisitsByContact(int contactId)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Verificar que el contacto pertenece al usuario
                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.ContactId == contactId && c.UserId == userId);
                
                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                var visits = await _context.Visits
                    .Include(v => v.User)
                    .Include(v => v.Contact)
                    .Where(v => v.ContactId == contactId && v.UserId == userId)
                    .OrderByDescending(v => v.ScheduledDate)
                    .Select(v => new
                    {
                        v.VisitId,
                        v.UserId,
                        v.ContactId,
                        v.ScheduledDate,
                        v.Address,
                        v.Notes,
                        v.Status,
                        v.CompletedAt,
                        v.Tema,
                        v.CreatedAt,
                        UserName = v.User.FullName,
                        ContactName = v.Contact.FullName,
                        ContactPhone = v.Contact.Phone,
                        ContactCategory = v.Contact.Category
                    })
                    .ToListAsync();

                return Ok(visits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener visitas del contacto");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    // DTOs
    public class VisitCreateDto
    {
        public int ContactId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string? Tema { get; set; }
    }

    public class VisitUpdateDto
    {
        public int ContactId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string? Tema { get; set; }
    }

    public class CompleteVisitDto
    {
        public string? Notes { get; set; }
    }
}
'@

Write-Host "=== CONTROLADOR DE VISITAS ===" -ForegroundColor Green
Write-Host ""
Write-Host "4. Controllers/VisitsController.cs:" -ForegroundColor Yellow
Write-Host $visitsController

# Actualización del AppDbContext
$dbContextUpdate = @'
// Agregar estas líneas al AppDbContext.cs existente:

public DbSet<Contact> Contacts { get; set; }
public DbSet<Visit> Visits { get; set; }

// En el método OnModelCreating, agregar:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Configurar Contact
    modelBuilder.Entity<Contact>()
        .HasOne(c => c.User)
        .WithMany()
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);
    
    // Configurar Visit
    modelBuilder.Entity<Visit>()
        .HasOne(v => v.User)
        .WithMany()
        .HasForeignKey(v => v.UserId)
        .OnDelete(DeleteBehavior.Restrict);
        
    modelBuilder.Entity<Visit>()
        .HasOne(v => v.Contact)
        .WithMany()
        .HasForeignKey(v => v.ContactId)
        .OnDelete(DeleteBehavior.Cascade);
}
'@

Write-Host ""
Write-Host "=== ACTUALIZACIÓN DEL DBCONTEXT ===" -ForegroundColor Cyan
Write-Host $dbContextUpdate

Write-Host ""
Write-Host "=== RESUMEN COMPLETO ===" -ForegroundColor Green
Write-Host "✅ Modelos: Contact.cs y Visit.cs"
Write-Host "✅ Controladores: ContactsController.cs y VisitsController.cs"
Write-Host "✅ DTOs incluidos para crear/actualizar"
Write-Host "✅ Validación de seguridad (solo datos del usuario logueado)"
Write-Host "Manejo de errores completo"