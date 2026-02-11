using Visitapp.Application.DTOs.Auth;
using Visitapp.Application.Interfaces;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Visitapp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordService passwordService,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLowerInvariant());
                
                if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
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
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                throw new InvalidOperationException("An error occurred during login");
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate password
                if (!_passwordService.IsValidPassword(request.Password))
                {
                    throw new ArgumentException("Password does not meet requirements");
                }

                // Check if user already exists
                if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email.ToLowerInvariant()))
                {
                    throw new InvalidOperationException("Email already registered");
                }

                if (await _unitOfWork.Users.ExistsByPhoneAsync(request.Phone))
                {
                    throw new InvalidOperationException("Phone already registered");
                }

                await _unitOfWork.BeginTransactionAsync();

                // Create user
                var passwordHash = _passwordService.HashPassword(request.Password);
                var user = User.Create(request.FullName, request.Email, request.Phone, passwordHash, request.ChurchId);

                var createdUser = await _unitOfWork.Users.CreateAsync(user);
                await _unitOfWork.SaveChangesAsync();

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
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                throw new InvalidOperationException("An error occurred during registration");
            }
        }

        public async Task<UserResponse> GetCurrentUserAsync(int userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetWithRolesAsync(userId);
                
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "user";

                return new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsVerified = user.IsVerified,
                    CreatedAt = user.CreatedAt,
                    Role = role,
                    ChurchId = user.ChurchId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user: {UserId}", userId);
                throw new InvalidOperationException("An error occurred while retrieving user information");
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = _tokenService.ValidateToken(token);
                if (principal == null) return false;

                var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId)) return false;

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }
    }
}