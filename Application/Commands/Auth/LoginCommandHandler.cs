using Microsoft.Extensions.Logging;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Application.Interfaces;
using Visitapp.Domain.Interfaces;

namespace Visitapp.Application.Commands.Auth
{
    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordService passwordService,
            ILogger<LoginCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task<AuthResponse> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(command.Email.ToLowerInvariant());
                
                if (user == null || !_passwordService.VerifyPassword(command.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", command.Email);
                    throw new UnauthorizedAccessException("Invalid credentials");
                }

                var userWithRoles = await _unitOfWork.Users.GetWithRolesAsync(user.Id);
                var role = userWithRoles?.UserRoles.FirstOrDefault()?.Role?.Name ?? "user";

                var token = _tokenService.GenerateJwtToken(user, role);
                var expiresAt = _tokenService.GetTokenExpiration(token);

                _logger.LogInformation("Successful login for user: {UserId}", user.Id);

                return new AuthResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    Message = "Login successful",
                    User = new UserResponse
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        IsVerified = user.IsVerified,
                        CreatedAt = user.CreatedAt,
                        Role = role,
                        ChurchId = user.ChurchId
                    }
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", command.Email);
                throw new InvalidOperationException("An error occurred during login");
            }
        }
    }
}