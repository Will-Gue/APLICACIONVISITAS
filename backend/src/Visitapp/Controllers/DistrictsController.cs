using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictsController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<DistrictsController> _logger;

        public DistrictsController(VisitAppContext context, ILogger<DistrictsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistrictDto>>> GetDistricts()
        {
            try
            {
                var districts = await _context.Districts
                    .Select(d => new DistrictDto
                    {
                        DistrictId = d.DistrictId,
                        DistrictName = d.DistrictName,
                        IsActive = d.IsActive,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener distritos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DistrictDto>> GetDistrict(int id)
        {
            try
            {
                var district = await _context.Districts
                    .Where(d => d.DistrictId == id)
                    .Select(d => new DistrictDto
                    {
                        DistrictId = d.DistrictId,
                        DistrictName = d.DistrictName,
                        IsActive = d.IsActive,
                        CreatedAt = d.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (district == null)
                    return NotFound(new { message = "Distrito no encontrado" });

                return Ok(district);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener distrito {DistrictId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<DistrictDto>> PostDistrict(DistrictCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.DistrictName))
                    return BadRequest(new { message = "El nombre del distrito es requerido" });

                if (await _context.Districts.AnyAsync(d => d.DistrictName == dto.DistrictName))
                    return Conflict(new { message = "El nombre del distrito ya existe" });

                var district = new Districts
                {
                    DistrictName = dto.DistrictName.Trim(),
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Districts.Add(district);
                await _context.SaveChangesAsync();

                var districtDto = new DistrictDto
                {
                    DistrictId = district.DistrictId,
                    DistrictName = district.DistrictName,
                    IsActive = district.IsActive,
                    CreatedAt = district.CreatedAt
                };

                return CreatedAtAction(nameof(GetDistrict), new { id = district.DistrictId }, districtDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear distrito");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDistrict(int id, DistrictUpdateDto dto)
        {
            try
            {
                var district = await _context.Districts.FindAsync(id);
                if (district == null)
                    return NotFound(new { message = "Distrito no encontrado" });

                if (string.IsNullOrWhiteSpace(dto.DistrictName))
                    return BadRequest(new { message = "El nombre del distrito es requerido" });

                var nameExists = await _context.Districts
                    .AnyAsync(d => d.DistrictName == dto.DistrictName && d.DistrictId != id);
                if (nameExists)
                    return Conflict(new { message = "El nombre del distrito ya existe" });

                district.DistrictName = dto.DistrictName.Trim();
                district.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar distrito {DistrictId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistrict(int id)
        {
            try
            {
                var district = await _context.Districts.FindAsync(id);
                if (district == null)
                    return NotFound(new { message = "Distrito no encontrado" });

                _context.Districts.Remove(district);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar distrito {DistrictId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}