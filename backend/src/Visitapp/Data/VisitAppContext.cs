using Microsoft.EntityFrameworkCore;
using Visitapp.Domain.Entities;
using Visitapp.Domain.Entities;

namespace Visitapp.Data
{
    public class VisitAppContext : DbContext
    {
        public VisitAppContext(DbContextOptions<VisitAppContext> options) : base(options)
        {
        }

        // Clean Architecture entities (only source of truth)
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Visitapp.Domain.Entities.Church> Churches { get; set; }
        public DbSet<District> Districts { get; set; }
        
        // Other entities - TODO: Migrate these to Domain.Entities
        // public DbSet<Notifications> Notifications { get; set; }
        // public DbSet<Temas> Temas { get; set; }
        // public DbSet<PreguntasClaves> PreguntasClaves { get; set; }
        // public DbSet<UserDistricts> UserDistricts { get; set; }

        // Auditoría
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure all entities with Clean Architecture approach
            ConfigureCleanArchitectureEntities(modelBuilder);
        }

        private void ConfigureCleanArchitectureEntities(ModelBuilder modelBuilder)
        {
            // Configure Clean Architecture User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Single table name for both
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.IsVerified).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Phone).IsUnique();
                
                entity.HasOne(e => e.Church)
                    .WithMany(c => c.Users)
                    .HasForeignKey(e => e.ChurchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure other Clean Architecture entities with clean table names
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Contacts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.ToTable("Visits");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.ScheduledDate).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Visits)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Contact)
                    .WithMany(c => c.Visits)
                    .HasForeignKey(e => e.ContactId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => new { e.UserId, e.RoleId });
                
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Visitapp.Domain.Entities.Church>(entity =>
            {
                entity.ToTable("Churches");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasOne(e => e.District)
                    .WithMany(d => d.Churches)
                    .HasForeignKey(e => e.DistrictId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("Districts");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Auditoría de acciones de usuario
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.AuditLogId);
                entity.Property(e => e.UserId).HasMaxLength(100);
                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Module).HasMaxLength(100);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Details).HasMaxLength(1000);
            });
        }
    }
}