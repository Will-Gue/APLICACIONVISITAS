using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Visitapp.Data;
using Visitapp.Models;
using Visitapp.Application.DTOs.Auth;
using System.Net.Mail;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Visitapp.Application.Interfaces.IEmailService _emailService;

        public AuthController(VisitAppContext context, ILogger<AuthController> logger, IConfiguration configuration, Visitapp.Application.Interfaces.IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                    return BadRequest(new { message = "Email y contraseña son requeridos" });

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLower().Trim());

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                    return Unauthorized(new { message = "Credenciales incorrectas" });

                // Obtener el rol del usuario
                var userRole = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .FirstOrDefaultAsync(ur => ur.UserId == user.UserId);

                string role = userRole?.Role?.RoleName ?? "user";

                var token = GenerateJwtToken(user, role);

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsVerified = user.IsVerified,
                    CreatedAt = user.CreatedAt,
                    Role = role
                };

                return Ok(new
                {
                    token = token,
                    user = userDto,
                    message = "Login exitoso"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para email {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registerDto.Email) || !IsValidEmail(registerDto.Email))
                    return BadRequest(new { message = "Email no válido" });

                if (string.IsNullOrWhiteSpace(registerDto.Password) || registerDto.Password.Length < 6)
                    return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres" });

                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email.ToLower().Trim()))
                    return Conflict(new { message = "Email ya registrado" });

                if (await _context.Users.AnyAsync(u => u.Phone == registerDto.Phone))
                    return Conflict(new { message = "Teléfono ya registrado" });

                var user = new Users
                {
                    FullName = registerDto.FullName.Trim(),
                    Email = registerDto.Email.Trim().ToLower(),
                    Phone = registerDto.Phone.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsVerified = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Asignar rol por defecto
                var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "user");
                if (defaultRole != null)
                {
                    var userRole = new UserRoles
                    {
                        UserId = user.UserId,
                        RoleId = defaultRole.RoleId,
                        AssignedDate = DateTime.UtcNow
                    };
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                // Notificar a todos los administradores por email (sin duplicados)
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName.ToLower() == "admin");
                if (adminRole != null)
                {
                    var adminUserIds = await _context.UserRoles
                        .Where(ur => ur.RoleId == adminRole.RoleId)
                        .Select(ur => ur.UserId)
                        .ToListAsync();
                    var adminEmails = await _context.Users
                        .Where(u => adminUserIds.Contains(u.UserId) && !string.IsNullOrEmpty(u.Email))
                        .Select(u => u.Email)
                        .Distinct()
                        .ToListAsync();
                    if (adminEmails.Count > 0)
                    {
                        var subject = "Nuevo registro de usuario en VisitApp";
                        var body = $"<b>Nuevo usuario registrado:</b><br>Nombre: {user.FullName}<br>Email: {user.Email}<br>Teléfono: {user.Phone}<br>Fecha: {user.CreatedAt:yyyy-MM-dd HH:mm} UTC";
                        foreach (var adminEmail in adminEmails)
                        {
                            try
                            {
                                await _emailService.SendEmailAsync(adminEmail, subject, body);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error enviando email de notificación a admin {adminEmail}");
                            }
                        }
                    }
                }

                var token = GenerateJwtToken(user, "user");

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsVerified = user.IsVerified,
                    CreatedAt = user.CreatedAt,
                    Role = "user"
                };

                return Ok(new
                {
                    token = token,
                    user = userDto,
                    message = "Registro exitoso"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro para email {Email}", registerDto.Email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "Token inválido" });

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                var userRole = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .FirstOrDefaultAsync(ur => ur.UserId == user.UserId);

                string role = userRole?.Role?.RoleName ?? "user";

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsVerified = user.IsVerified,
                    CreatedAt = user.CreatedAt,
                    Role = role
                };

                return Ok(new { user = userDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario actual");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        private string GenerateJwtToken(Users user, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-long-and-secure-123456789");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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