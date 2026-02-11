        /// <summary>
        /// Método auxiliar para disparar notificaciones de visitas próximas y recordatorios.
        /// No modifica la lógica de visitas ni afecta flujos actuales.
        /// Pensado para ser llamado por tareas programadas, jobs externos o integración futura.
        /// </summary>
        /// <remarks>
        /// Ejemplo de uso: enviar notificaciones a usuarios con visitas en las próximas 24h.
        /// </remarks>
        /// <param name="hoursAhead">Horas hacia adelante para buscar visitas próximas (por defecto 24h)</param>
        /// <returns>Cantidad de notificaciones generadas</returns>
        [NonAction]
        public async Task<int> DispararNotificacionesVisitasProximasAsync(int hoursAhead = 24)
        {
            var ahora = DateTime.UtcNow;
            var hasta = ahora.AddHours(hoursAhead);
            // Buscar visitas programadas en el rango
            var visitas = await _context.Visits
                .Where(v => v.ScheduledDate >= ahora && v.ScheduledDate <= hasta && v.Status == "Pendiente")
                .Include(v => v.User)
                .Include(v => v.Contact)
                .ToListAsync();

            int notificacionesEnviadas = 0;
            foreach (var visita in visitas)
            {
                if (visita.User == null || string.IsNullOrEmpty(visita.User.Email))
                    continue;

                var mensaje = $"Tienes una visita programada con {visita.Contact?.FullName ?? "contacto"} el {visita.ScheduledDate:yyyy-MM-dd HH:mm}.";

                // Crear notificación en base de datos
                var notificacion = new Notifications
                {
                    UserId = visita.UserId,
                    VisitId = visita.VisitId,
                    Type = "email",
                    Message = mensaje,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Notifications.Add(notificacion);

                // Enviar email (opcional, se puede comentar si solo se quiere push/local)
                try
                {
                    await _emailService.SendEmailAsync(visita.User.Email, "Recordatorio de visita próxima", mensaje);
                    notificacionesEnviadas++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error enviando email de recordatorio a {visita.User.Email}");
                }
            }
            await _context.SaveChangesAsync();
            return notificacionesEnviadas;
        }
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visitapp.Data;
using Visitapp.Models;

using Visitapp.Dtos;
using Visitapp.Application.Interfaces;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class NotificationsController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IEmailService _emailService;

        public NotificationsController(VisitAppContext context, ILogger<NotificationsController> logger, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Visit)
                        .ThenInclude(v => v!.Contact)
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        VisitId = n.VisitId,
                        Type = n.Type,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        UserName = n.User != null ? n.User.FullName : null,
                        ContactName = n.Visit != null && n.Visit.Contact != null ? n.Visit.Contact.FullName : null,
                        VisitDate = n.Visit != null ? n.Visit.ScheduledDate : null
                    })
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Visit)
                        .ThenInclude(v => v!.Contact)
                    .Where(n => n.NotificationId == id)
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        VisitId = n.VisitId,
                        Type = n.Type,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        UserName = n.User != null ? n.User.FullName : null,
                        ContactName = n.Visit != null && n.Visit.Contact != null ? n.Visit.Contact.FullName : null,
                        VisitDate = n.Visit != null ? n.Visit.ScheduledDate : null
                    })
                    .FirstOrDefaultAsync();

                if (notification == null)
                    return NotFound(new { message = "Notificación no encontrada" });

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificación {NotificationId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> PostNotification(NotificationCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Type))
                    return BadRequest(new { message = "El tipo de notificación es requerido" });

                if (string.IsNullOrWhiteSpace(dto.Message))
                    return BadRequest(new { message = "El mensaje es requerido" });


                // Verificar que el usuario existe y obtener email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.UserId);
                if (user == null)
                    return BadRequest(new { message = "El usuario especificado no existe" });

                // Verificar que la visita existe (si se proporciona)
                if (dto.VisitId.HasValue)
                {
                    var visitExists = await _context.Visits.AnyAsync(v => v.VisitId == dto.VisitId.Value);
                    if (!visitExists)
                        return BadRequest(new { message = "La visita especificada no existe" });
                }


                var notification = new Notifications
                {
                    UserId = dto.UserId,
                    VisitId = dto.VisitId,
                    Type = dto.Type.Trim(),
                    Message = dto.Message.Trim(),
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Enviar email si el tipo es 'email' y el usuario tiene email
                if (notification.Type.ToLower() == "email" && !string.IsNullOrWhiteSpace(user.Email))
                {
                    try
                    {
                        await _emailService.SendEmailAsync(user.Email, "Nueva notificación", notification.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar email de notificación a {Email}", user.Email);
                        // No interrumpir la creación de la notificación por error de email
                    }
                }

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Obtener la notificación creada con información relacionada
                var createdNotification = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Visit)
                        .ThenInclude(v => v!.Contact)
                    .Where(n => n.NotificationId == notification.NotificationId)
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        VisitId = n.VisitId,
                        Type = n.Type,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        UserName = n.User != null ? n.User.FullName : null,
                        ContactName = n.Visit != null && n.Visit.Contact != null ? n.Visit.Contact.FullName : null,
                        VisitDate = n.Visit != null ? n.Visit.ScheduledDate : null
                    })
                    .FirstAsync();

                return CreatedAtAction(nameof(GetNotification), new { id = notification.NotificationId }, createdNotification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear notificación");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, NotificationUpdateDto dto)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                    return NotFound(new { message = "Notificación no encontrada" });

                notification.IsRead = dto.IsRead;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar notificación {NotificationId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                    return NotFound(new { message = "Notificación no encontrada" });

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar notificación {NotificationId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsByUser(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Visit)
                        .ThenInclude(v => v!.Contact)
                    .Where(n => n.UserId == userId)
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        VisitId = n.VisitId,
                        Type = n.Type,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        UserName = n.User != null ? n.User.FullName : null,
                        ContactName = n.Visit != null && n.Visit.Contact != null ? n.Visit.Contact.FullName : null,
                        VisitDate = n.Visit != null ? n.Visit.ScheduledDate : null
                    })
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotificationsByUser(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Include(n => n.User)
                    .Include(n => n.Visit)
                        .ThenInclude(v => v!.Contact)
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        UserId = n.UserId,
                        VisitId = n.VisitId,
                        Type = n.Type,
                        Message = n.Message,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        UserName = n.User != null ? n.User.FullName : null,
                        ContactName = n.Visit != null && n.Visit.Contact != null ? n.Visit.Contact.FullName : null,
                        VisitDate = n.Visit != null ? n.Visit.ScheduledDate : null
                    })
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener notificaciones no leídas del usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("user/{userId}/mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Se marcaron {notifications.Count} notificaciones como leídas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar todas las notificaciones como leídas para el usuario {UserId}", userId);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}