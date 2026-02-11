using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Visitapp.Data;
using Visitapp.Application.Interfaces;
using Visitapp.Application.Services;
using Visitapp.Application.Commands;
using Visitapp.Application.Commands.Auth;
using Visitapp.Application.DTOs.Auth;
using Visitapp.Application.Common;
using Visitapp.Infrastructure.Services;
using Visitapp.Infrastructure.Repositories;
using Visitapp.Infrastructure.Common;
using Visitapp.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configurar para HTTP en desarrollo
builder.WebHost.UseUrls("http://0.0.0.0:5254");

// Configurar Kestrel para mejor manejo de conexiones
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxConcurrentConnections = 100;
    serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50MB
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VisitApp API", Version = "v1" });
    c.SwaggerDoc("v2", new() { Title = "VisitApp API v2 (Clean Architecture)", Version = "v2" });
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "your-secret-key-here-make-it-long-and-secure-123456789")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Entity Framework with Performance Optimizations
builder.Services.AddDbContext<VisitAppContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => 
        {
            sqlServerOptions.CommandTimeout(30);
            sqlServerOptions.EnableRetryOnFailure(3);
            sqlServerOptions.MigrationsAssembly("Visitapp");
        })
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableServiceProviderCaching()
    .EnableDetailedErrors(builder.Environment.IsDevelopment()));

// Add Memory Cache for Performance
builder.Services.AddMemoryCache();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// Register Cache Service
builder.Services.AddScoped<Visitapp.Application.Interfaces.ICacheService, Visitapp.Infrastructure.Services.CacheService>();

// Register Clean Architecture Dependencies with Design Patterns

// Domain Layer - Repositories with Specification Pattern
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Layer - Command Handlers (Command Pattern)
builder.Services.AddScoped<ICommandHandler<LoginCommand, AuthResponse>, LoginCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RegisterCommand, AuthResponse>, RegisterCommandHandler>();

// Application Layer - Mediator Pattern
builder.Services.AddScoped<IMediator, Visitapp.Infrastructure.Common.Mediator>();

// Application Layer - Services (Service Layer Pattern)
builder.Services.AddScoped<IAuthService, AuthService>();

// Infrastructure Layer - Services (Strategy Pattern)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Email Service for SMTP notifications
builder.Services.AddScoped<Visitapp.Application.Interfaces.IEmailService, Visitapp.Infrastructure.Services.EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VisitApp API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "VisitApp API v2 (Clean Architecture)");
        c.DefaultModelsExpandDepth(-1);
    });
}

// Usar CORS
// Performance Middleware
app.UseResponseCompression();

// Security Middleware
app.UseMiddleware<Visitapp.Middleware.SecurityHeadersMiddleware>();
app.UseMiddleware<Visitapp.Middleware.RateLimitingMiddleware>();

app.UseCors("AllowAll");

// Comentar para desarrollo HTTP
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program class accessible for testing
public partial class Program { }