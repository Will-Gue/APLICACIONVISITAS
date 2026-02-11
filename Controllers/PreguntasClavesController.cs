using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasClavesController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<PreguntasClavesController> _logger;

        public PreguntasClavesController(VisitAppContext context, ILogger<PreguntasClavesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/PreguntasClaves
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreguntaClaveDto>>> GetPreguntasClaves()
        {
            try
            {
                var preguntas = await _context.PreguntasClaves
                    // DESCOMENTAR cuando tengas el modelo User:
                    // .Include(p => p.Creator)
                    // .Include(p => p.Tema)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Categoria)
                    .ThenBy(p => p.OrdenSecuencia)
                    .Select(p => new PreguntaClaveDto
                    {
                        PreguntaId = p.PreguntaId,
                        TemaId = p.TemaId,
                        // DESCOMENTAR cuando tengas el modelo Temas con relación:
                        // TemaTitulo = p.Tema != null ? p.Tema.TituloTema : null,
                        TemaTitulo = null,
                        Pregunta = p.Pregunta,
                        Respuesta = p.Respuesta,
                        ReferenciaBiblica = p.ReferenciaBiblica,
                        Categoria = p.Categoria,
                        OrdenSecuencia = p.OrdenSecuencia,
                        CreatedBy = p.CreatedBy,
                        // DESCOMENTAR cuando tengas el modelo User:
                        // CreatedByName = p.Creator != null ? p.Creator.FullName : null,
                        CreatedByName = null,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener preguntas clave");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/PreguntasClaves/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PreguntaClaveDto>> GetPreguntaClave(int id)
        {
            try
            {
                var pregunta = await _context.PreguntasClaves
                    // DESCOMENTAR cuando tengas los modelos:
                    // .Include(p => p.Creator)
                    // .Include(p => p.Tema)
                    .Where(p => p.PreguntaId == id)
                    .Select(p => new PreguntaClaveDto
                    {
                        PreguntaId = p.PreguntaId,
                        TemaId = p.TemaId,
                        // TemaTitulo = p.Tema != null ? p.Tema.TituloTema : null,
                        TemaTitulo = null,
                        Pregunta = p.Pregunta,
                        Respuesta = p.Respuesta,
                        ReferenciaBiblica = p.ReferenciaBiblica,
                        Categoria = p.Categoria,
                        OrdenSecuencia = p.OrdenSecuencia,
                        CreatedBy = p.CreatedBy,
                        // CreatedByName = p.Creator != null ? p.Creator.FullName : null,
                        CreatedByName = null,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (pregunta == null)
                    return NotFound(new { message = "Pregunta clave no encontrada" });

                return Ok(pregunta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pregunta clave {PreguntaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/PreguntasClaves/tema/5
        [HttpGet("tema/{temaId}")]

                // GET: api/PreguntasClaves/user?pageNumber=1&pageSize=10&searchTerm=
                [HttpGet("user")]
                public async Task<ActionResult<PagedResultDto<PreguntaClaveDto>>> GetPreguntasClavesPaginadas(
                    [FromQuery] int pageNumber = 1,
                    [FromQuery] int pageSize = 10,
                    [FromQuery] string? searchTerm = null)
                {
                    try
                    {
                        if (pageNumber < 1) pageNumber = 1;
                        if (pageSize < 1 || pageSize > 100) pageSize = 10;

                        var query = _context.PreguntasClaves.Where(p => p.IsActive);

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            query = query.Where(p =>
                                p.Pregunta.Contains(searchTerm) ||
                                (p.Respuesta != null && p.Respuesta.Contains(searchTerm)) ||
                                (p.ReferenciaBiblica != null && p.ReferenciaBiblica.Contains(searchTerm)) ||
                                (p.Categoria != null && p.Categoria.Contains(searchTerm)));
                        }

                        var totalCount = await query.CountAsync();
                        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                        var preguntas = await query
                            .OrderBy(p => p.Categoria)
                            .ThenBy(p => p.OrdenSecuencia)
                            .ThenBy(p => p.Pregunta)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .Select(p => new PreguntaClaveDto
                            {
                                PreguntaId = p.PreguntaId,
                                TemaId = p.TemaId,
                                TemaTitulo = null,
                                Pregunta = p.Pregunta,
                                Respuesta = p.Respuesta,
                                ReferenciaBiblica = p.ReferenciaBiblica,
                                Categoria = p.Categoria,
                                OrdenSecuencia = p.OrdenSecuencia,
                                CreatedBy = p.CreatedBy,
                                CreatedByName = null,
                                IsActive = p.IsActive,
                                CreatedAt = p.CreatedAt,
                                UpdatedAt = p.UpdatedAt
                            })
                            .ToListAsync();

                        return Ok(new PagedResultDto<PreguntaClaveDto>
                        {
                            PageNumber = pageNumber,
                            PageSize = pageSize,
                            TotalCount = totalCount,
                            TotalPages = totalPages,
                            Data = preguntas
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al obtener preguntas clave paginadas");
                        return StatusCode(500, new { message = "Error interno del servidor" });
                    }
                }
        public async Task<ActionResult<IEnumerable<PreguntaClaveDto>>> GetPreguntasByTema(int temaId)
        {
            try
            {
                var preguntas = await _context.PreguntasClaves
                    // .Include(p => p.Creator)
                    .Where(p => p.TemaId == temaId && p.IsActive)
                    .OrderBy(p => p.OrdenSecuencia)
                    .Select(p => new PreguntaClaveDto
                    {
                        PreguntaId = p.PreguntaId,
                        TemaId = p.TemaId,
                        Pregunta = p.Pregunta,
                        Respuesta = p.Respuesta,
                        ReferenciaBiblica = p.ReferenciaBiblica,
                        Categoria = p.Categoria,
                        OrdenSecuencia = p.OrdenSecuencia,
                        CreatedBy = p.CreatedBy,
                        // CreatedByName = p.Creator != null ? p.Creator.FullName : null,
                        CreatedByName = null,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener preguntas por tema {TemaId}", temaId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/PreguntasClaves/categoria/Doctrinal
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<PreguntaClaveDto>>> GetPreguntasByCategoria(string categoria)
        {
            try
            {
                var preguntas = await _context.PreguntasClaves
                    // .Include(p => p.Creator)
                    // .Include(p => p.Tema)
                    .Where(p => p.Categoria == categoria && p.IsActive)
                    .OrderBy(p => p.OrdenSecuencia)
                    .Select(p => new PreguntaClaveDto
                    {
                        PreguntaId = p.PreguntaId,
                        TemaId = p.TemaId,
                        // TemaTitulo = p.Tema != null ? p.Tema.TituloTema : null,
                        TemaTitulo = null,
                        Pregunta = p.Pregunta,
                        Respuesta = p.Respuesta,
                        ReferenciaBiblica = p.ReferenciaBiblica,
                        Categoria = p.Categoria,
                        OrdenSecuencia = p.OrdenSecuencia,
                        CreatedBy = p.CreatedBy,
                        // CreatedByName = p.Creator != null ? p.Creator.FullName : null,
                        CreatedByName = null,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener preguntas por categoría {Categoria}", categoria);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/PreguntasClaves/search?term=Trinidad
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PreguntaClaveDto>>> SearchPreguntas([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest(new { message = "El término de búsqueda es requerido" });

                var preguntas = await _context.PreguntasClaves
                    // .Include(p => p.Creator)
                    .Where(p => p.IsActive &&
                        (p.Pregunta.Contains(term) ||
                         (p.Respuesta != null && p.Respuesta.Contains(term)) ||
                         (p.ReferenciaBiblica != null && p.ReferenciaBiblica.Contains(term))))
                    .OrderBy(p => p.Pregunta)
                    .Select(p => new PreguntaClaveDto
                    {
                        PreguntaId = p.PreguntaId,
                        TemaId = p.TemaId,
                        Pregunta = p.Pregunta,
                        Respuesta = p.Respuesta,
                        ReferenciaBiblica = p.ReferenciaBiblica,
                        Categoria = p.Categoria,
                        OrdenSecuencia = p.OrdenSecuencia,
                        CreatedBy = p.CreatedBy,
                        // CreatedByName = p.Creator != null ? p.Creator.FullName : null,
                        CreatedByName = null,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar preguntas con término {Term}", term);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/PreguntasClaves/categorias
        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<PreguntaClaveCategoriaDto>>> GetCategorias()
        {
            try
            {
                var categorias = await _context.PreguntasClaves
                    .Where(p => p.IsActive)
                    .GroupBy(p => p.Categoria)
                    .Select(g => new PreguntaClaveCategoriaDto
                    {
                        Categoria = g.Key,
                        CantidadPreguntas = g.Count()
                    })
                    .OrderBy(c => c.Categoria)
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías de preguntas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/PreguntasClaves/stats
        [HttpGet("stats")]
        public async Task<ActionResult<PreguntasClavesStatsDto>> GetStats()
        {
            try
            {
                var preguntas = await _context.PreguntasClaves
                    .Where(p => p.IsActive)
                    .ToListAsync();

                var stats = new PreguntasClavesStatsDto
                {
                    TotalPreguntas = preguntas.Count,
                    PreguntasConReferencia = preguntas.Count(p => !string.IsNullOrEmpty(p.ReferenciaBiblica)),
                    TotalCategorias = preguntas.Select(p => p.Categoria).Distinct().Count(),
                    PreguntasConTema = preguntas.Count(p => p.TemaId.HasValue)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de preguntas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/PreguntasClaves
        [HttpPost]
        public async Task<ActionResult<PreguntaClaveDto>> CreatePregunta([FromBody] CreatePreguntaClaveDto dto)
        {
            try
            {
                var pregunta = new PreguntasClaves
                {
                    TemaId = dto.TemaId,
                    Pregunta = dto.Pregunta,
                    Respuesta = dto.Respuesta,
                    ReferenciaBiblica = dto.ReferenciaBiblica,
                    Categoria = dto.Categoria,
                    OrdenSecuencia = dto.OrdenSecuencia,
                    CreatedBy = dto.CreatedBy,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.PreguntasClaves.Add(pregunta);
                await _context.SaveChangesAsync();

                var preguntaDto = new PreguntaClaveDto
                {
                    PreguntaId = pregunta.PreguntaId,
                    TemaId = pregunta.TemaId,
                    Pregunta = pregunta.Pregunta,
                    Respuesta = pregunta.Respuesta,
                    ReferenciaBiblica = pregunta.ReferenciaBiblica,
                    Categoria = pregunta.Categoria,
                    OrdenSecuencia = pregunta.OrdenSecuencia,
                    CreatedBy = pregunta.CreatedBy,
                    IsActive = pregunta.IsActive,
                    CreatedAt = pregunta.CreatedAt,
                    UpdatedAt = pregunta.UpdatedAt
                };

                return CreatedAtAction(nameof(GetPreguntaClave), new { id = pregunta.PreguntaId }, preguntaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pregunta clave");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/PreguntasClaves/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePregunta(int id, [FromBody] UpdatePreguntaClaveDto dto)
        {
            try
            {
                var pregunta = await _context.PreguntasClaves.FindAsync(id);

                if (pregunta == null)
                    return NotFound(new { message = "Pregunta clave no encontrada" });

                pregunta.TemaId = dto.TemaId;
                pregunta.Pregunta = dto.Pregunta;
                pregunta.Respuesta = dto.Respuesta;
                pregunta.ReferenciaBiblica = dto.ReferenciaBiblica;
                pregunta.Categoria = dto.Categoria;
                pregunta.OrdenSecuencia = dto.OrdenSecuencia;
                pregunta.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Pregunta clave actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar pregunta {PreguntaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/PreguntasClaves/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePregunta(int id)
        {
            try
            {
                var pregunta = await _context.PreguntasClaves.FindAsync(id);

                if (pregunta == null)
                    return NotFound(new { message = "Pregunta clave no encontrada" });

                pregunta.IsActive = false;
                pregunta.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Pregunta clave eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pregunta {PreguntaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/PreguntasClaves/bulk (Crear múltiples preguntas a la vez)
        [HttpPost("bulk")]
        public async Task<ActionResult> CreatePreguntasBulk([FromBody] List<CreatePreguntaClaveDto> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return BadRequest(new { message = "Debe proporcionar al menos una pregunta" });

                var preguntas = dtos.Select(dto => new PreguntasClaves
                {
                    TemaId = dto.TemaId,
                    Pregunta = dto.Pregunta,
                    Respuesta = dto.Respuesta,
                    ReferenciaBiblica = dto.ReferenciaBiblica,
                    Categoria = dto.Categoria,
                    OrdenSecuencia = dto.OrdenSecuencia,
                    CreatedBy = dto.CreatedBy,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();

                _context.PreguntasClaves.AddRange(preguntas);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{preguntas.Count} preguntas creadas exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear preguntas en bulk");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/PreguntasClaves/reorder (Reordenar preguntas)
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderPreguntas([FromBody] List<ReorderPreguntaDto> dtos)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return BadRequest(new { message = "Debe proporcionar al menos una pregunta para reordenar" });

                foreach (var dto in dtos)
                {
                    var pregunta = await _context.PreguntasClaves.FindAsync(dto.PreguntaId);
                    if (pregunta != null)
                    {
                        pregunta.OrdenSecuencia = dto.NuevoOrden;
                        pregunta.UpdatedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Preguntas reordenadas exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reordenar preguntas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}