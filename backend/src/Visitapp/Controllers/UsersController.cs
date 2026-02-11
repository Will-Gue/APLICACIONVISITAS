using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Models;
using Visitapp.Dtos;
using System.Net.Mail;
using Visitapp.Application.Common.Interfaces;
using Visitapp.Domain.Models;
using System.Security.Claims;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IPasswordService _passwordService;
        private readonly Visitapp.Application.Interfaces.IEmailService _emailService;

        public UsersController(VisitAppContext context, ILogger<UsersController> logger, IAuditLogRepository auditLogRepository, IPasswordService passwordService, Visitapp.Application.Interfaces.IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _passwordService = passwordService;
            _emailService = emailService;
        }
        // PATCH: api/Users/{id}/change-password
        [HttpPatch("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] UserChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (!_passwordService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Contraseña actual incorrecta" });

            if (!_passwordService.IsValidPassword(dto.NewPassword))
                return BadRequest(new { message = "La nueva contraseña no cumple los requisitos de seguridad" });

            user.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            await _auditLogRepository.AddAsync(new AuditLog {
                UserId = GetCurrentUserId(),
                Action = "CHANGE_PASSWORD",
                Module = "Users",
                Date = DateTime.UtcNow,
                Details = $"Contraseña cambiada: id={id}"
            });
            return Ok(new { message = "Contraseña actualizada correctamente" });
        }

        // PATCH: api/Users/{id}/change-email
        [HttpPatch("{id}/change-email")]
        public async Task<IActionResult> ChangeEmail(int id, [FromBody] UserChangeEmailDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (!_passwordService.VerifyPassword(dto.Password, user.PasswordHash))
                return BadRequest(new { message = "Contraseña incorrecta" });

            if (string.IsNullOrWhiteSpace(dto.NewEmail) || !IsValidEmail(dto.NewEmail))
                return BadRequest(new { message = "Email no válido" });

            if (await _context.Users.AnyAsync(u => u.Email == dto.NewEmail && u.UserId != id))
                return Conflict(new { message = "Email ya registrado por otro usuario" });

            user.Email = dto.NewEmail.Trim().ToLower();
            await _context.SaveChangesAsync();

            await _auditLogRepository.AddAsync(new AuditLog {
                UserId = GetCurrentUserId(),
                Action = "CHANGE_EMAIL",
                Module = "Users",
                Date = DateTime.UtcNow,
                Details = $"Email cambiado: id={id}, nuevo={user.Email}"
            });
            return Ok(new { message = "Email actualizado correctamente" });
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone,
                        IsVerified = u.IsVerified,
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();

                // Auditoría: consulta de usuarios
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "GET_ALL",
                    Module = "Users",
                    Date = DateTime.UtcNow,
                    Details = $"Consulta de todos los usuarios ({users.Count} registros)"
                });
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserId == id)
                    .Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone,
                        IsVerified = u.IsVerified,
                        CreatedAt = u.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                // Auditoría: consulta de usuario por id
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "GET_BY_ID",
                    Module = "Users",
                    Date = DateTime.UtcNow,
                    Details = $"Consulta de usuario id={id}"
                });
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(UserCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                    return BadRequest(new { message = "Email no válido" });

                if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                    return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres" });

                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                    return Conflict(new { message = "Email ya registrado" });

                if (await _context.Users.AnyAsync(u => u.Phone == dto.Phone))
                    return Conflict(new { message = "Teléfono ya registrado" });

                // Evitar duplicar notificación si el usuario ya existe
                var alreadyNotified = await _context.Users.AnyAsync(u => u.Email == dto.Email);
                if (!alreadyNotified)
                {
                    // Obtener email del administrador desde configuración o base de datos
                    var adminEmail = "admin@visitapp.com"; // Reemplazar por lógica real si es necesario
                    var subject = "Nuevo usuario registrado en VisitApp";
                    var body = $@"<b>Nuevo usuario registrado:</b><br>
                        Nombre: {dto.FullName}<br>
                        Email: {dto.Email}<br>
                        Teléfono: {dto.Phone}<br>
                        <br>Asigne rol desde el panel de administración.";
                    try
                    {
                        await _emailService.SendEmailAsync(adminEmail, subject, body);
                        _logger.LogInformation("Notificación de nuevo registro enviada a {AdminEmail}", adminEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error enviando notificación de nuevo registro");
                    }
                }

                var user = new Users
                {
                    FullName = dto.FullName.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    Phone = dto.Phone.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsVerified = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsVerified = user.IsVerified,
                    CreatedAt = user.CreatedAt
                };

                // Auditoría: creación de usuario
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "CREATE",
                    Module = "Users",
                    Date = DateTime.UtcNow,
                    Details = $"Usuario creado: id={user.UserId}, email={user.Email}"
                });
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserUpdateDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                // Seguridad: Solo el usuario autenticado puede editar su perfil
                var currentUserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == null || user.UserId.ToString() != currentUserId)
                    return Forbid("No tiene permiso para editar este perfil");

                if (string.IsNullOrWhiteSpace(dto.FullName))
                    return BadRequest(new { message = "El nombre es requerido" });

                // Validar que el teléfono no esté en uso por otro usuario
                if (!string.IsNullOrWhiteSpace(dto.Phone))
                {
                    var phoneExists = await _context.Users
                        .AnyAsync(u => u.Phone == dto.Phone && u.UserId != id);
                    if (phoneExists)
                        return Conflict(new { message = "El teléfono ya está en uso por otro usuario" });
                }

                user.FullName = dto.FullName.Trim();
                user.Phone = dto.Phone?.Trim() ?? user.Phone;
                user.IsVerified = dto.IsVerified;

                await _context.SaveChangesAsync();
                // Auditoría: actualización de usuario
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "UPDATE",
                    Module = "Users",
                    Date = DateTime.UtcNow,
                    Details = $"Usuario actualizado: id={id}"
                });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
            /// <summary>
            /// PUT /api/Users/{id}
            /// Solo el usuario autenticado puede editar su perfil.
            /// </summary>
            /// <remarks>
            /// Ejemplo de request:
            ///     PUT /api/Users/1
            ///     {
            ///         "fullName": "Familia Pérez",
            ///         "phone": "5551234567",
            ///         "isVerified": true
            ///     }
            /// </remarks>
            /// <response code="204">Actualización exitosa</response>
            /// <response code="403">No tiene permiso</response>
            /// <response code="404">No encontrado</response>
            /// <response code="400">Datos inválidos</response>
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                // Auditoría: eliminación de usuario
                await _auditLogRepository.AddAsync(new AuditLog {
                    UserId = GetCurrentUserId(),
                    Action = "DELETE",
                    Module = "Users",
                    Date = DateTime.UtcNow,
                    Details = $"Usuario eliminado: id={id}"
                });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}