using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<UserRolesController> _logger;

        public UserRolesController(VisitAppContext context, ILogger<UserRolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserRoles()
        {
            try
            {
                var userRoles = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Select(ur => new UserRoleDto
                    {
                        UserId = ur.UserId,
                        RoleId = ur.RoleId,
                        AssignedDate = ur.AssignedDate,
                        UserName = ur.User != null ? ur.User.FullName : null,
                        UserEmail = ur.User != null ? ur.User.Email : null,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        RoleDescription = ur.Role != null ? ur.Role.Description : null
                    })
                    .OrderBy(ur => ur.UserName)
                    .ThenBy(ur => ur.RoleName)
                    .ToListAsync();

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones usuario-rol");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetRolesByUser(int userId)
        {
            try
            {
                var userRoles = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => new UserRoleDto
                    {
                        UserId = ur.UserId,
                        RoleId = ur.RoleId,
                        AssignedDate = ur.AssignedDate,
                        UserName = ur.User != null ? ur.User.FullName : null,
                        UserEmail = ur.User != null ? ur.User.Email : null,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        RoleDescription = ur.Role != null ? ur.Role.Description : null
                    })
                    .OrderBy(ur => ur.RoleName)
                    .ToListAsync();

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUsersByRole(int roleId)
        {
            try
            {
                var userRoles = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => new UserRoleDto
                    {
                        UserId = ur.UserId,
                        RoleId = ur.RoleId,
                        AssignedDate = ur.AssignedDate,
                        UserName = ur.User != null ? ur.User.FullName : null,
                        UserEmail = ur.User != null ? ur.User.Email : null,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        RoleDescription = ur.Role != null ? ur.Role.Description : null
                    })
                    .OrderBy(ur => ur.UserName)
                    .ToListAsync();

                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios con rol {RoleId}", roleId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserRoleDto>> PostUserRole(UserRoleCreateDto dto)
        {
            try
            {
                // Verificar que el usuario existe
                var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
                if (!userExists)
                    return BadRequest(new { message = "El usuario especificado no existe" });

                // Verificar que el rol existe
                var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == dto.RoleId);
                if (!roleExists)
                    return BadRequest(new { message = "El rol especificado no existe" });

                // Verificar que la relación no existe ya
                var relationExists = await _context.UserRoles
                    .AnyAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);
                if (relationExists)
                    return Conflict(new { message = "El usuario ya tiene asignado este rol" });

                var userRole = new UserRoles
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId,
                    AssignedDate = DateTime.UtcNow
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                // Obtener la relación creada con información completa
                var createdUserRole = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId)
                    .Select(ur => new UserRoleDto
                    {
                        UserId = ur.UserId,
                        RoleId = ur.RoleId,
                        AssignedDate = ur.AssignedDate,
                        UserName = ur.User != null ? ur.User.FullName : null,
                        UserEmail = ur.User != null ? ur.User.Email : null,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        RoleDescription = ur.Role != null ? ur.Role.Description : null
                    })
                    .FirstAsync();

                return Ok(createdUserRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar rol a usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserRole(UserRoleDeleteDto dto)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);

                if (userRole == null)
                    return NotFound(new { message = "La relación usuario-rol no existe" });

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar relación usuario-rol");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{userId}/{roleId}")]
        public async Task<IActionResult> DeleteUserRoleByIds(int userId, int roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole == null)
                    return NotFound(new { message = "La relación usuario-rol no existe" });

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar relación usuario-rol {UserId}/{RoleId}", userId, roleId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> PostMultipleUserRoles(List<UserRoleCreateDto> userRoleDtos)
        {
            try
            {
                var createdUserRoles = new List<UserRoleDto>();

                foreach (var dto in userRoleDtos)
                {
                    // Verificar que el usuario existe
                    var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
                    if (!userExists)
                        return BadRequest(new { message = $"El usuario con ID {dto.UserId} no existe" });

                    // Verificar que el rol existe
                    var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == dto.RoleId);
                    if (!roleExists)
                        return BadRequest(new { message = $"El rol con ID {dto.RoleId} no existe" });

                    // Verificar que la relación no existe ya
                    var relationExists = await _context.UserRoles
                        .AnyAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);
                    if (relationExists)
                        continue; // Saltar si ya existe

                    var userRole = new UserRoles
                    {
                        UserId = dto.UserId,
                        RoleId = dto.RoleId,
                        AssignedDate = DateTime.UtcNow
                    };

                    _context.UserRoles.Add(userRole);
                }

                await _context.SaveChangesAsync();

                // Obtener todas las relaciones creadas
                var allUserRoles = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Where(ur => userRoleDtos.Any(dto => dto.UserId == ur.UserId && dto.RoleId == ur.RoleId))
                    .Select(ur => new UserRoleDto
                    {
                        UserId = ur.UserId,
                        RoleId = ur.RoleId,
                        AssignedDate = ur.AssignedDate,
                        UserName = ur.User != null ? ur.User.FullName : null,
                        UserEmail = ur.User != null ? ur.User.Email : null,
                        RoleName = ur.Role != null ? ur.Role.RoleName : null,
                        RoleDescription = ur.Role != null ? ur.Role.Description : null
                    })
                    .ToListAsync();

                return Ok(allUserRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar múltiples roles");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("user/{userId}/check-role/{roleId}")]
        public async Task<ActionResult<bool>> CheckUserHasRole(int userId, int roleId)
        {
            try
            {
                var hasRole = await _context.UserRoles
                    .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                return Ok(new { hasRole });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si usuario {UserId} tiene rol {RoleId}", userId, roleId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}