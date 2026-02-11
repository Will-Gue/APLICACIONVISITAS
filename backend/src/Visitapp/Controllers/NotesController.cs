using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<NotesController> _logger;

        public NotesController(VisitAppContext context, ILogger<NotesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notes>>> GetNotesByUser(int userId)
        {
            try
            {
                var notes = await _context.Notes
                    .Where(n => n.UserId == userId)
                    .ToListAsync();
                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notas del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Notes>> PostNote(Notes note)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(note.Content))
                    return BadRequest(new { message = "El contenido de la nota es requerido" });
                note.CreatedAt = DateTime.UtcNow;
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetNotesByUser), new { userId = note.UserId }, note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nota");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, Notes note)
        {
            try
            {
                var existingNote = await _context.Notes.FindAsync(id);
                if (existingNote == null)
                    return NotFound(new { message = "Nota no encontrada" });
                if (string.IsNullOrWhiteSpace(note.Content))
                    return BadRequest(new { message = "El contenido de la nota es requerido" });
                existingNote.Content = note.Content.Trim();
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar nota {NoteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            try
            {
                var note = await _context.Notes.FindAsync(id);
                if (note == null)
                    return NotFound(new { message = "Nota no encontrada" });
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar nota {NoteId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
