using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Models;
using Visitapp.Dtos;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChurchesController(VisitAppContext context, ILogger<ChurchesController> logger) : ControllerBase
    {
        private readonly VisitAppContext _context = context;
        private readonly ILogger<ChurchesController> _logger = logger;

        // GET: api/Churches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChurchDto>>> GetChurches()
        {
            try
            {
                var churches = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.District!.DistrictName)
                    .ThenBy(c => c.ChurchName)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(churches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener iglesias");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Churches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChurchDto>> GetChurch(int id)
        {
            try
            {
                var church = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.ChurchId == id)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (church == null)
                    return NotFound(new { message = "Iglesia no encontrada" });

                return Ok(church);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener iglesia {ChurchId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Churches/district/5
        [HttpGet("district/{districtId}")]
        public async Task<ActionResult<IEnumerable<ChurchDto>>> GetChurchesByDistrict(int districtId)
        {
            try
            {
                var churches = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.DistrictId == districtId && c.IsActive)
                    .OrderBy(c => c.ChurchName)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(churches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener iglesias del distrito {DistrictId}", districtId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Churches/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult<IEnumerable<ChurchStatisticsDto>>> GetChurchStatistics()
        {
            try
            {
                var statistics = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.IsActive)
                    .Select(c => new ChurchStatisticsDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        TotalUsers = c.Users != null ? c.Users.Count : 0,
                        TotalPastores = c.Users != null ? c.Users.Count(u => u.UserRoles!.Any(ur => ur.RoleId == 13)) : 0,
                        TotalLideres = c.Users != null ? c.Users.Count(u => u.UserRoles!.Any(ur => ur.RoleId == 14)) : 0,
                        TotalFamilias = c.Users != null ? c.Users.Count(u => u.UserRoles!.Any(ur => ur.RoleId == 15)) : 0,
                        IsActive = c.IsActive
                    })
                    .OrderBy(c => c.DistrictName)
                    .ThenBy(c => c.ChurchName)
                    .ToListAsync();

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de iglesias");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Churches/statistics/5
        [HttpGet("statistics/{id}")]
        public async Task<ActionResult<ChurchStatisticsDto>> GetChurchStatisticsById(int id)
        {
            try
            {
                var statistics = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.ChurchId == id)
                    .Select(c => new ChurchStatisticsDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        TotalUsers = c.Users != null ? c.Users.Count : 0,
                        TotalPastores = c.Users != null ? c.Users.Count(u => u.UserRoles!.Any(ur => ur.RoleId == 13)) : 0,
                        TotalLideres = c.Users != null ? c.Users.Count(u => u.UserRoles!.Any(ur => ur.RoleId == 14)) : 0,
                        TotalFamilias = c.Users != null ? c.Users.Count(u => u.UserRoles!.Any(ur => ur.RoleId == 15)) : 0,
                        IsActive = c.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (statistics == null)
                    return NotFound(new { message = "Iglesia no encontrada" });

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de iglesia {ChurchId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/Churches
        [HttpPost]
        public async Task<ActionResult<ChurchDto>> PostChurch(ChurchCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ChurchName))
                    return BadRequest(new { message = "El nombre de la iglesia es requerido" });

                // Verificar que el distrito existe
                var districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == dto.DistrictId);
                if (!districtExists)
                    return BadRequest(new { message = "El distrito especificado no existe" });

                // Verificar que no existe otra iglesia con el mismo nombre en el mismo distrito
                var churchExists = await _context.Churches
                    .AnyAsync(c => c.ChurchName == dto.ChurchName && c.DistrictId == dto.DistrictId);
                if (churchExists)
                    return BadRequest(new { message = "Ya existe una iglesia con ese nombre en el distrito especificado" });

                var church = new Church
                {
                    ChurchName = dto.ChurchName,
                    DistrictId = dto.DistrictId,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.Churches.Add(church);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Cargar la iglesia con su distrito
                var churchDto = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.ChurchId == church.ChurchId)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                return CreatedAtAction(nameof(GetChurch), new { id = church.ChurchId }, churchDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al crear iglesia");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/Churches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChurch(int id, ChurchUpdateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var church = await _context.Churches.FindAsync(id);
                if (church == null)
                    return NotFound(new { message = "Iglesia no encontrada" });

                if (string.IsNullOrWhiteSpace(dto.ChurchName))
                    return BadRequest(new { message = "El nombre de la iglesia es requerido" });

                // Verificar que el distrito existe
                var districtExists = await _context.Districts.AnyAsync(d => d.DistrictId == dto.DistrictId);
                if (!districtExists)
                    return BadRequest(new { message = "El distrito especificado no existe" });

                // Verificar que no existe otra iglesia con el mismo nombre en el mismo distrito
                var churchExists = await _context.Churches
                    .AnyAsync(c => c.ChurchName == dto.ChurchName
                                && c.DistrictId == dto.DistrictId
                                && c.ChurchId != id);
                if (churchExists)
                    return BadRequest(new { message = "Ya existe otra iglesia con ese nombre en el distrito especificado" });

                church.ChurchName = dto.ChurchName;
                church.DistrictId = dto.DistrictId;
                church.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Cargar la iglesia actualizada con su distrito
                var churchDto = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.ChurchId == id)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                return Ok(churchDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!ChurchExists(id))
                    return NotFound(new { message = "Iglesia no encontrada" });
                else
                    throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar iglesia {ChurchId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/Churches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChurch(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var church = await _context.Churches.FindAsync(id);
                if (church == null)
                    return NotFound(new { message = "Iglesia no encontrada" });

                // Verificar si hay usuarios asignados a esta iglesia
                var hasUsers = await _context.Users.AnyAsync(u => u.ChurchId == id);
                if (hasUsers)
                    return BadRequest(new { message = "No se puede eliminar la iglesia porque tiene usuarios asignados" });

                // Soft delete
                church.IsActive = false;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Iglesia eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar iglesia {ChurchId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/Churches/5/toggle
        [HttpPost("{id}/toggle")]
        public async Task<ActionResult<ChurchDto>> ToggleChurchStatus(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var church = await _context.Churches.FindAsync(id);
                if (church == null)
                    return NotFound(new { message = "Iglesia no encontrada" });

                church.IsActive = !church.IsActive;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Cargar la iglesia con su distrito
                var churchDto = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.ChurchId == id)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                return Ok(churchDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cambiar estado de iglesia {ChurchId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Churches/search?term=ejemplo
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ChurchDto>>> SearchChurches([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest(new { message = "El término de búsqueda es requerido" });

                var churches = await _context.Churches
                    .Include(c => c.District)
                    .Where(c => c.IsActive &&
                           (c.ChurchName.Contains(term) ||
                            (c.District != null && c.District.DistrictName.Contains(term))))
                    .OrderBy(c => c.District!.DistrictName)
                    .ThenBy(c => c.ChurchName)
                    .Select(c => new ChurchDto
                    {
                        ChurchId = c.ChurchId,
                        ChurchName = c.ChurchName,
                        DistrictId = c.DistrictId,
                        DistrictName = c.District != null ? c.District.DistrictName : null,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(churches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar iglesias con término {SearchTerm}", term);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        private bool ChurchExists(int id)
        {
            return _context.Churches.Any(e => e.ChurchId == id);
        }
    }
}