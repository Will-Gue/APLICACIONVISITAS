using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDistrictsController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<UserDistrictsController> _logger;

        public UserDistrictsController(VisitAppContext context, ILogger<UserDistrictsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDistrictDto>>> GetUserDistricts()
        {
            try
            {
                var userDistricts = await _context.UserDistricts
                    .Include(ud => ud.User)
                    .Include(ud => ud.District)
                    .Where(ud => ud.IsActive)
                    .Select(ud => new UserDistrictDto
                    {
                        UserId = ud.UserId,
                        DistrictId = ud.DistrictId,
                        AssignedDate = ud.AssignedDate,
                        IsActive = ud.IsActive,
                        UserName = ud.User != null ? ud.User.FullName : null,
                        UserEmail = ud.User != null ? ud.User.Email : null,
                        DistrictName = ud.District != null ? ud.District.DistrictName : null
                    })
                    .OrderBy(ud => ud.UserName)
                    .ThenBy(ud => ud.DistrictName)
                    .ToListAsync();

                return Ok(userDistricts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener relaciones usuario-distrito");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserDistrictDto>>> GetDistrictsByUser(int userId)
        {
            try
            {
                var userDistricts = await _context.UserDistricts
                    .Include(ud => ud.User)
                    .Include(ud => ud.District)
                    .Where(ud => ud.UserId == userId && ud.IsActive)
                    .Select(ud => new UserDistrictDto
                    {
                        UserId = ud.UserId,
                        DistrictId = ud.DistrictId,
                        AssignedDate = ud.AssignedDate,
                        IsActive = ud.IsActive,
                        UserName = ud.User != null ? ud.User.FullName : null,
                        UserEmail = ud.User != null ? ud.User.Email : null,
                        DistrictName = ud.District != null ? ud.District.DistrictName : null
                    })
                    .OrderBy(ud => ud.DistrictName)
                    .ToListAsync();

                return Ok(userDistricts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener distritos del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("district/{districtId}")]
        public async Task<ActionResult<IEnumerable<UserDistrictDto>>> GetUsersByDistrict(int districtId)
        {
            try
            {
                var userDistricts = await _context.UserDistricts
                    .Include(ud => ud.User)
                    .Include(ud => ud.District)
                    .Where(ud => ud.DistrictId == districtId && ud.IsActive)
                    .Select(ud => new UserDistrictDto
                    {
                        UserId = ud.UserId,
                        DistrictId = ud.DistrictId,
                        AssignedDate = ud.AssignedDate,
                        IsActive = ud.IsActive,
                        UserName = ud.User != null ? ud.User.FullName : null,
                        UserEmail = ud.User != null ? ud.User.Email : null,
                        DistrictName = ud.District != null ? ud.District.DistrictName : null
                    })
                    .OrderBy(ud => ud.UserName)
                    .ToListAsync();

                return Ok(userDistricts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios del distrito {DistrictId}", districtId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDistrictDto>> PostUserDistrict(UserDistrictCreateDto dto)
        {
            try
            {
                // Verificar que el usuario existe
                var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
                if (!userExists)
                    return BadRequest(new { message = "El usuario especificado no existe" });

                // Verificar que el distrito existe
                var districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == dto.DistrictId);
                if (!districtExists)
                    return BadRequest(new { message = "El distrito especificado no existe" });

                // Verificar que la relación no existe ya
                var relationExists = await _context.UserDistricts
                    .AnyAsync(ud => ud.UserId == dto.UserId && ud.DistrictId == dto.DistrictId);
                if (relationExists)
                    return Conflict(new { message = "El usuario ya está asignado a este distrito" });

                var userDistrict = new UserDistricts
                {
                    UserId = dto.UserId,
                    DistrictId = dto.DistrictId,
                    AssignedDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.UserDistricts.Add(userDistrict);
                await _context.SaveChangesAsync();

                // Obtener la relación creada con información completa
                var createdUserDistrict = await _context.UserDistricts
                    .Include(ud => ud.User)
                    .Include(ud => ud.District)
                    .Where(ud => ud.UserId == userDistrict.UserId && ud.DistrictId == userDistrict.DistrictId)
                    .Select(ud => new UserDistrictDto
                    {
                        UserId = ud.UserId,
                        DistrictId = ud.DistrictId,
                        AssignedDate = ud.AssignedDate,
                        IsActive = ud.IsActive,
                        UserName = ud.User != null ? ud.User.FullName : null,
                        UserEmail = ud.User != null ? ud.User.Email : null,
                        DistrictName = ud.District != null ? ud.District.DistrictName : null
                    })
                    .FirstAsync();

                return Ok(createdUserDistrict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar distrito a usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{userId}/{districtId}")]
        public async Task<IActionResult> PutUserDistrict(int userId, int districtId, UserDistrictUpdateDto dto)
        {
            try
            {
                var userDistrict = await _context.UserDistricts
                    .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DistrictId == districtId);

                if (userDistrict == null)
                    return NotFound(new { message = "La relación usuario-distrito no existe" });

                userDistrict.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar relación usuario-distrito {UserId}/{DistrictId}", userId, districtId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{userId}/{districtId}")]
        public async Task<IActionResult> DeleteUserDistrict(int userId, int districtId)
        {
            try
            {
                var userDistrict = await _context.UserDistricts
                    .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DistrictId == districtId);

                if (userDistrict == null)
                    return NotFound(new { message = "La relación usuario-distrito no existe" });

                // Soft delete
                userDistrict.IsActive = false;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar relación usuario-distrito {UserId}/{DistrictId}", userId, districtId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<UserDistrictDto>>> PostMultipleUserDistricts(List<UserDistrictCreateDto> userDistrictDtos)
        {
            try
            {
                foreach (var dto in userDistrictDtos)
                {
                    // Verificar que el usuario existe
                    var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
                    if (!userExists)
                        return BadRequest(new { message = $"El usuario con ID {dto.UserId} no existe" });

                    // Verificar que el distrito existe
                    var districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == dto.DistrictId);
                    if (!districtExists)
                        return BadRequest(new { message = $"El distrito con ID {dto.DistrictId} no existe" });

                    // Verificar que la relación no existe ya
                    var relationExists = await _context.UserDistricts
                        .AnyAsync(ud => ud.UserId == dto.UserId && ud.DistrictId == dto.DistrictId);
                    if (relationExists)
                        continue; // Saltar si ya existe

                    var userDistrict = new UserDistricts
                    {
                        UserId = dto.UserId,
                        DistrictId = dto.DistrictId,
                        AssignedDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.UserDistricts.Add(userDistrict);
                }

                await _context.SaveChangesAsync();

                // Obtener todas las relaciones creadas
                var allUserDistricts = await _context.UserDistricts
                    .Include(ud => ud.User)
                    .Include(ud => ud.District)
                    .Where(ud => userDistrictDtos.Any(dto => dto.UserId == ud.UserId && dto.DistrictId == ud.DistrictId) && ud.IsActive)
                    .Select(ud => new UserDistrictDto
                    {
                        UserId = ud.UserId,
                        DistrictId = ud.DistrictId,
                        AssignedDate = ud.AssignedDate,
                        IsActive = ud.IsActive,
                        UserName = ud.User != null ? ud.User.FullName : null,
                        UserEmail = ud.User != null ? ud.User.Email : null,
                        DistrictName = ud.District != null ? ud.District.DistrictName : null
                    })
                    .ToListAsync();

                return Ok(allUserDistricts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar múltiples distritos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("user/{userId}/check-district/{districtId}")]
        public async Task<ActionResult<bool>> CheckUserHasDistrict(int userId, int districtId)
        {
            try
            {
                var hasDistrict = await _context.UserDistricts
                    .AnyAsync(ud => ud.UserId == userId && ud.DistrictId == districtId && ud.IsActive);

                return Ok(new { hasDistrict });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si usuario {UserId} tiene distrito {DistrictId}", userId, districtId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}