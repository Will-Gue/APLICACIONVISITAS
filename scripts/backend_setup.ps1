# Crear endpoints de Contactos y Visitas para el backend C# .NET

# Crear el modelo Contact
$contactModel = @'
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitApp.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navegación
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        
        [NotMapped]
        public string? UserName => User?.FullName;
    }
}
'@

# Crear el modelo Visit
$visitModel = @'
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitApp.Models
{
    public class Visit
    {
        [Key]
        public int VisitId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int ContactId { get; set; }
        
        [Required]
        public DateTime ScheduledDate { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Programada";
        
        public DateTime? CompletedAt { get; set; }
        
        [StringLength(100)]
        public string? Tema { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navegación
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        
        [ForeignKey("ContactId")]
        public Contact Contact { get; set; } = null!;
        
        [NotMapped]
        public string? UserName => User?.FullName;
        
        [NotMapped]
        public string? ContactName => Contact?.FullName;
        
        [NotMapped]
        public string? ContactPhone => Contact?.Phone;
        
        [NotMapped]
        public string? ContactCategory => Contact?.Category;
    }
}
'@

# Crear el controlador de Contactos
$contactsController = @'
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
    public class ContactsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(AppDbContext context, ILogger<ContactsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetContacts()
        {
            try
            {
                var userId = GetCurrentUserId();
                var contacts = await _context.Contacts
                    .Include(c => c.User)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new
                    {
                        c.ContactId,
                        c.UserId,
                        c.FullName,
                        c.Phone,
                        c.Email,
                        c.Category,
                        c.CreatedAt,
                        UserName = c.User.FullName
                    })
                    .ToListAsync();

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contactos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/contacts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetContact(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var contact = await _context.Contacts
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.ContactId == id && c.UserId == userId);

                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                var result = new
                {
                    contact.ContactId,
                    contact.UserId,
                    contact.FullName,
                    contact.Phone,
                    contact.Email,
                    contact.Category,
                    contact.CreatedAt,
                    UserName = contact.User.FullName
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contacto {ContactId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/contacts
        [HttpPost]
        public async Task<ActionResult<object>> CreateContact(ContactCreateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var contact = new Contact
                {
                    UserId = userId,
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Category = dto.Category,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Recargar con datos de navegación
                await _context.Entry(contact).Reference(c => c.User).LoadAsync();

                var result = new
                {
                    contact.ContactId,
                    contact.UserId,
                    contact.FullName,
                    contact.Phone,
                    contact.Email,
                    contact.Category,
                    contact.CreatedAt,
                    UserName = contact.User.FullName
                };

                return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear contacto");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/contacts/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateContact(int id, ContactUpdateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var contact = await _context.Contacts
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.ContactId == id && c.UserId == userId);

                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                contact.FullName = dto.FullName;
                contact.Phone = dto.Phone;
                contact.Email = dto.Email;
                contact.Category = dto.Category;

                await _context.SaveChangesAsync();

                var result = new
                {
                    contact.ContactId,
                    contact.UserId,
                    contact.FullName,
                    contact.Phone,
                    contact.Email,
                    contact.Category,
                    contact.CreatedAt,
                    UserName = contact.User.FullName
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar contacto {ContactId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/contacts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(c => c.ContactId == id && c.UserId == userId);

                if (contact == null)
                    return NotFound(new { message = "Contacto no encontrado" });

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Contacto eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar contacto {ContactId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/contacts/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchContacts([FromQuery] string term)
        {
            try
            {
                var userId = GetCurrentUserId();
                var contacts = await _context.Contacts
                    .Include(c => c.User)
                    .Where(c => c.UserId == userId && (
                        c.FullName.Contains(term) ||
                        (c.Phone != null && c.Phone.Contains(term)) ||
                        (c.Email != null && c.Email.Contains(term))
                    ))
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new
                    {
                        c.ContactId,
                        c.UserId,
                        c.FullName,
                        c.Phone,
                        c.Email,
                        c.Category,
                        c.CreatedAt,
                        UserName = c.User.FullName
                    })
                    .ToListAsync();

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar contactos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/contacts/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<object>>> GetContactsByCategory(string category)
        {
            try
            {
                var userId = GetCurrentUserId();
                var contacts = await _context.Contacts
                    .Include(c => c.User)
                    .Where(c => c.UserId == userId && c.Category == category)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new
                    {
                        c.ContactId,
                        c.UserId,
                        c.FullName,
                        c.Phone,
                        c.Email,
                        c.Category,
                        c.CreatedAt,
                        UserName = c.User.FullName
                    })
                    .ToListAsync();

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contactos por categoría");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    // DTOs
    public class ContactCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Category { get; set; }
    }

    public class ContactUpdateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Category { get; set; }
    }
}
'@

Write-Host "=== ARCHIVOS PARA EL BACKEND C# .NET ===" -ForegroundColor Green
Write-Host ""
Write-Host "1. Models/Contact.cs:" -ForegroundColor Yellow
Write-Host $contactModel
Write-Host ""
Write-Host "2. Models/Visit.cs:" -ForegroundColor Yellow  
Write-Host $visitModel
Write-Host ""
Write-Host "3. Controllers/ContactsController.cs:" -ForegroundColor Yellow
Write-Host $contactsController

Write-Host ""
Write-Host "=== INSTRUCCIONES ===" -ForegroundColor Cyan
Write-Host "1. Copia estos archivos en tu proyecto backend C# .NET"
Write-Host "2. Ejecuta las migraciones para crear las tablas:"
Write-Host "   Add-Migration AddContactsAndVisits"
Write-Host "   Update-Database"
Write-Host "3. Asegúrate de que AppDbContext incluya los DbSet para Contact y Visit"
Write-Host "4. Reinicia el servidor backend"