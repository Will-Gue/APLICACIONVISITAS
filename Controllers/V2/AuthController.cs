using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Visitapp.Application.Commands.Auth;
using Visitapp.Application.Common;
using Visitapp.Application.DTOs.Auth;

namespace Visitapp.Controllers.V2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate user with email and password using Command Pattern
        /// </summary>
        /// <param name="command">Login command with credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _mediator.SendAsync(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized login attempt for email: {Email}", command.Email);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Authentication Failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", command.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An error occurred while processing your request",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Register a new user account using Command Pattern
        /// </summary>
        /// <param name="command">Registration command with user information</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _mediator.SendAsync(command);
                return CreatedAtAction(nameof(GetCurrentUser), new { }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid registration data for email: {Email} - {Error}", command.Email, ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Data",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already"))
            {
                _logger.LogWarning("Duplicate registration attempt for email: {Email}", command.Email);
                return Conflict(new ProblemDetails
                {
                    Title = "Conflict",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", command.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An error occurred while processing your request",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Get current authenticated user information
        /// </summary>
        /// <returns>Current user details</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ProblemDetails
                    {
                        Title = "Invalid Token",
                        Detail = "Token does not contain valid user information",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

                // This would be implemented as a Query using CQRS pattern
                // For now, we'll use a simple approach
                return Ok(new { message = "User profile endpoint - implement with Query pattern" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An error occurred while processing your request",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}