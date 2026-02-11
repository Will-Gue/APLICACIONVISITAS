using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Domain.Entities;
using Visitapp.Application.DTOs;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemasController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<TemasController> _logger;

        public TemasController(VisitAppContext context, ILogger<TemasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Temas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemaDto>>> GetTemas()
        {
            try
            {
                var temas = await _context.Temas
                    // .Include(t => t.Creator)  // DESCOMENTAR cuando tengas el modelo User
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.OrdenSecuencia)
                    .ThenByDescending(t => t.CreatedAt)
                    .Select(t => new TemaDto
                    {
                        TemaId = t.TemaId,
                        TituloTema = t.TituloTema,
                        Descripcion = t.Descripcion,
                        Categoria = t.Categoria,
                        Contenido = t.Contenido,
                        ReferenciasBasicas = t.ReferenciasBasicas,
                        PdfPath = t.PdfPath,
                        PdfFileName = t.PdfFileName,
                        PdfSize = t.PdfSize,
                        Duracion = t.Duracion,
                        Nivel = t.Nivel,
                        OrdenSecuencia = t.OrdenSecuencia,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = null, // t.Creator != null ? t.Creator.FullName : null,
                        EsPublico = t.EsPublico,
                        IsActive = t.IsActive,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(temas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener temas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Temas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemaDto>> GetTema(int id)
        {
            try
            {
                var tema = await _context.Temas
                    // .Include(t => t.Creator)  // DESCOMENTAR cuando tengas el modelo User
                    .Where(t => t.TemaId == id)
                    .Select(t => new TemaDto
                    {
                        TemaId = t.TemaId,
                        TituloTema = t.TituloTema,
                        Descripcion = t.Descripcion,
                        Categoria = t.Categoria,
                        Contenido = t.Contenido,
                        ReferenciasBasicas = t.ReferenciasBasicas,
                        PdfPath = t.PdfPath,
                        PdfFileName = t.PdfFileName,
                        PdfSize = t.PdfSize,
                        Duracion = t.Duracion,
                        Nivel = t.Nivel,
                        OrdenSecuencia = t.OrdenSecuencia,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = null, // t.Creator != null ? t.Creator.FullName : null,
                        EsPublico = t.EsPublico,
                        IsActive = t.IsActive,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (tema == null)
                    return NotFound(new { message = "Tema no encontrado" });

                return Ok(tema);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tema {TemaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Temas/categoria/Doctrinal
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<TemaDto>>> GetTemasByCategoria(string categoria)
        {
            try
            {
                var temas = await _context.Temas
                    // .Include(t => t.Creator)  // DESCOMENTAR cuando tengas el modelo User
                    .Where(t => t.Categoria == categoria && t.IsActive)
                    .OrderBy(t => t.OrdenSecuencia)
                    .ThenBy(t => t.TituloTema)
                    .Select(t => new TemaDto
                    {
                        TemaId = t.TemaId,
                        TituloTema = t.TituloTema,
                        Descripcion = t.Descripcion,
                        Categoria = t.Categoria,
                        ReferenciasBasicas = t.ReferenciasBasicas,
                        PdfPath = t.PdfPath,
                        PdfFileName = t.PdfFileName,
                        PdfSize = t.PdfSize,
                        Duracion = t.Duracion,
                        Nivel = t.Nivel,
                        OrdenSecuencia = t.OrdenSecuencia,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = null, // t.Creator != null ? t.Creator.FullName : null,
                        EsPublico = t.EsPublico,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(temas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener temas por categoría {Categoria}", categoria);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Temas/nivel/Basico
        [HttpGet("nivel/{nivel}")]
        public async Task<ActionResult<IEnumerable<TemaDto>>> GetTemasByNivel(string nivel)
        {
            try
            {
                var temas = await _context.Temas
                    // .Include(t => t.Creator)  // DESCOMENTAR cuando tengas el modelo User
                    .Where(t => t.Nivel == nivel && t.IsActive)
                    .OrderBy(t => t.OrdenSecuencia)
                    .ThenBy(t => t.TituloTema)
                    .Select(t => new TemaDto
                    {
                        TemaId = t.TemaId,
                        TituloTema = t.TituloTema,
                        Descripcion = t.Descripcion,
                        Categoria = t.Categoria,
                        ReferenciasBasicas = t.ReferenciasBasicas,
                        PdfPath = t.PdfPath,
                        PdfFileName = t.PdfFileName,
                        Duracion = t.Duracion,
                        Nivel = t.Nivel,
                        OrdenSecuencia = t.OrdenSecuencia,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = null, // t.Creator != null ? t.Creator.FullName : null,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return Ok(temas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener temas por nivel {Nivel}", nivel);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Temas/search?term=Trinidad
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TemaDto>>> SearchTemas([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest(new { message = "El término de búsqueda es requerido" });

                var temas = await _context.Temas
                    // .Include(t => t.Creator)  // DESCOMENTAR cuando tengas el modelo User
                    .Where(t => t.IsActive &&
                        (t.TituloTema.Contains(term) ||
                         (t.Descripcion != null && t.Descripcion.Contains(term)) ||
                         (t.ReferenciasBasicas != null && t.ReferenciasBasicas.Contains(term))))
                    .OrderBy(t => t.TituloTema)
                    .Select(t => new TemaDto
                    {
                        TemaId = t.TemaId,
                        TituloTema = t.TituloTema,
                        Descripcion = t.Descripcion,
                        Categoria = t.Categoria,
                        ReferenciasBasicas = t.ReferenciasBasicas,
                        PdfPath = t.PdfPath,
                        PdfFileName = t.PdfFileName,
                        Duracion = t.Duracion,
                        Nivel = t.Nivel,
                        OrdenSecuencia = t.OrdenSecuencia,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = null, // t.Creator != null ? t.Creator.FullName : null,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return Ok(temas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar temas con término {Term}", term);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Temas/categorias
        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<TemaCategoriaDto>>> GetCategorias()
        {
            try
            {
                var categorias = await _context.Temas
                    .Where(t => t.IsActive)
                    .GroupBy(t => t.Categoria)
                    .Select(g => new TemaCategoriaDto
                    {
                        Categoria = g.Key,
                        CantidadTemas = g.Count()
                    })
                    .OrderBy(c => c.Categoria)
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías de temas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Temas/stats
        [HttpGet("stats")]
        public async Task<ActionResult<TemasStatsDto>> GetStats()
        {
            try
            {
                var temas = await _context.Temas.Where(t => t.IsActive).ToListAsync();

                var stats = new TemasStatsDto
                {
                    TotalTemas = temas.Count,
                    TemasConPDF = temas.Count(t => !string.IsNullOrEmpty(t.PdfPath)),
                    TemasConTexto = temas.Count(t => !string.IsNullOrEmpty(t.Contenido)),
                    TotalCategorias = temas.Select(t => t.Categoria).Distinct().Count(),
                    TemasBasicos = temas.Count(t => t.Nivel == "Básico"),
                    TemasIntermedios = temas.Count(t => t.Nivel == "Intermedio"),
                    TemasAvanzados = temas.Count(t => t.Nivel == "Avanzado")
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de temas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // POST: api/Temas
        [HttpPost]
        public async Task<ActionResult<TemaDto>> CreateTema([FromBody] CreateTemaDto dto)
        {
            try
            {
                var tema = new Temas
                {
                    TituloTema = dto.TituloTema,
                    Descripcion = dto.Descripcion,
                    Categoria = dto.Categoria,
                    Contenido = dto.Contenido,
                    ReferenciasBasicas = dto.ReferenciasBasicas,
                    PdfPath = dto.PdfPath,
                    PdfFileName = dto.PdfFileName,
                    PdfSize = dto.PdfSize,
                    Duracion = dto.Duracion,
                    Nivel = dto.Nivel,
                    OrdenSecuencia = dto.OrdenSecuencia,
                    CreatedBy = dto.CreatedBy,
                    EsPublico = dto.EsPublico,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Temas.Add(tema);
                await _context.SaveChangesAsync();

                var temaDto = new TemaDto
                {
                    TemaId = tema.TemaId,
                    TituloTema = tema.TituloTema,
                    Descripcion = tema.Descripcion,
                    Categoria = tema.Categoria,
                    Contenido = tema.Contenido,
                    ReferenciasBasicas = tema.ReferenciasBasicas,
                    PdfPath = tema.PdfPath,
                    PdfFileName = tema.PdfFileName,
                    PdfSize = tema.PdfSize,
                    Duracion = tema.Duracion,
                    Nivel = tema.Nivel,
                    OrdenSecuencia = tema.OrdenSecuencia,
                    CreatedBy = tema.CreatedBy,
                    EsPublico = tema.EsPublico,
                    IsActive = tema.IsActive,
                    CreatedAt = tema.CreatedAt,
                    UpdatedAt = tema.UpdatedAt
                };

                return CreatedAtAction(nameof(GetTema), new { id = tema.TemaId }, temaDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tema");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/Temas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTema(int id, [FromBody] UpdateTemaDto dto)
        {
            try
            {
                var tema = await _context.Temas.FindAsync(id);

                if (tema == null)
                    return NotFound(new { message = "Tema no encontrado" });

                tema.TituloTema = dto.TituloTema;
                tema.Descripcion = dto.Descripcion;
                tema.Categoria = dto.Categoria;
                tema.Contenido = dto.Contenido;
                tema.ReferenciasBasicas = dto.ReferenciasBasicas;

                // Solo actualizar PDF si se proporciona uno nuevo
                if (!string.IsNullOrEmpty(dto.PdfPath))
                {
                    tema.PdfPath = dto.PdfPath;
                    tema.PdfFileName = dto.PdfFileName;
                    tema.PdfSize = dto.PdfSize;
                }

                tema.Duracion = dto.Duracion;
                tema.Nivel = dto.Nivel;
                tema.OrdenSecuencia = dto.OrdenSecuencia;
                tema.EsPublico = dto.EsPublico;
                tema.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Tema actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tema {TemaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/Temas/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTema(int id)
        {
            try
            {
                var tema = await _context.Temas.FindAsync(id);

                if (tema == null)
                    return NotFound(new { message = "Tema no encontrado" });

                tema.IsActive = false;
                tema.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Tema eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tema {TemaId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
