using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(VisitAppContext context, ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            try
            {
                var roles = await _context.Roles
                    .Select(r => new RoleDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Description = r.Description,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Where(r => r.RoleId == id)
                    .Select(r => new RoleDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Description = r.Description,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (role == null)
                    return NotFound(new { message = "Rol no encontrado" });

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol {RoleId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole(RoleCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.RoleName))
                    return BadRequest(new { message = "El nombre del rol es requerido" });

                if (await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                    return Conflict(new { message = "El nombre del rol ya existe" });

                var role = new Roles
                {
                    RoleName = dto.RoleName.Trim(),
                    Description = dto.Description?.Trim(),
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                var roleDto = new RoleDto
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedAt = role.CreatedAt
                };

                return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleUpdateDto dto)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                    return NotFound(new { message = "Rol no encontrado" });

                if (string.IsNullOrWhiteSpace(dto.RoleName))
                    return BadRequest(new { message = "El nombre del rol es requerido" });

                var nameExists = await _context.Roles
                    .AnyAsync(r => r.RoleName == dto.RoleName && r.RoleId != id);
                if (nameExists)
                    return Conflict(new { message = "El nombre del rol ya existe" });

                role.RoleName = dto.RoleName.Trim();
                role.Description = dto.Description?.Trim();
                role.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol {RoleId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                    return NotFound(new { message = "Rol no encontrado" });

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol {RoleId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}