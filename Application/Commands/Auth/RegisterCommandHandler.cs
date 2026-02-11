using Microsoft.Extensions.Logging;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Application.Interfaces;
using Visitapp.Domain.Builders;
using Visitapp.Domain.Interfaces;

namespace Visitapp.Application.Commands.Auth
{
    public class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordService passwordService,
            ILogger<RegisterCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task<AuthResponse> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate password using Strategy Pattern
                if (!_passwordService.IsValidPassword(command.Password))
                {
                    throw new ArgumentException("Password does not meet requirements");
                }

                // Check if user already exists
                if (await _unitOfWork.Users.ExistsByEmailAsync(command.Email.ToLowerInvariant()))
                {
                    throw new InvalidOperationException("Email already registered");
                }

                if (await _unitOfWork.Users.ExistsByPhoneAsync(command.Phone))
                {
                    throw new InvalidOperationException("Phone already registered");
                }

                await _unitOfWork.BeginTransactionAsync();

                // Create user using Builder Pattern
                var passwordHash = _passwordService.HashPassword(command.Password);
                var user = UserBuilder.New()
                    .WithFullName(command.FullName)
                    .WithEmail(command.Email)
                    .WithPhone(command.Phone)
                    .WithPasswordHash(passwordHash)
                    .WithChurch(command.ChurchId)
                    .AsUnverified()
                    .Build();

                var createdUser = await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Assign default role (this would need role management implementation)
                var role = "user";

                var token = _tokenService.GenerateJwtToken(createdUser, role);
                var expiresAt = _tokenService.GetTokenExpiration(token);

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("New user registered: {UserId}", createdUser.Id);

                return new AuthResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    Message = "Registration successful",
                    User = new UserResponse
                    {
                        Id = createdUser.Id,
                        FullName = createdUser.FullName,
                        Email = createdUser.Email,
                        Phone = createdUser.Phone,
                        IsVerified = createdUser.IsVerified,
                        CreatedAt = createdUser.CreatedAt,
                        Role = role,
                        ChurchId = createdUser.ChurchId
                    }
                };
            }
            catch (ArgumentException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            catch (InvalidOperationException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error during registration for email: {Email}", command.Email);
                throw new InvalidOperationException("An error occurred during registration");
            }
        }
    }
}