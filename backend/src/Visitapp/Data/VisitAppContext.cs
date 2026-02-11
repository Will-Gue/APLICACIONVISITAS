using Microsoft.EntityFrameworkCore;
using Visitapp.Models;
using Visitapp.Domain.Entities;

namespace Visitapp.Data
{
    public class VisitAppContext : DbContext
    {
        public VisitAppContext(DbContextOptions<VisitAppContext> options) : base(options)
        {
        }

        // Legacy entities (for existing controllers) - KEEP THESE
        public DbSet<Users> Users { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Visits> Visits { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Districts> Districts { get; set; }
        public DbSet<Visitapp.Models.Church> Churches { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Notes> Notes { get; set; }
        
        // Clean Architecture entities (for v2 controllers) - Different table names
        public DbSet<User> DomainUsers { get; set; }
        public DbSet<Contact> DomainContacts { get; set; }
        public DbSet<Visit> DomainVisits { get; set; }
        public DbSet<Role> DomainRoles { get; set; }
        public DbSet<UserRole> DomainUserRoles { get; set; }
        public DbSet<Visitapp.Domain.Entities.Church> DomainChurches { get; set; }
        public DbSet<District> DomainDistricts { get; set; }
        
        // Other entities
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Temas> Temas { get; set; }
        public DbSet<PreguntasClaves> PreguntasClaves { get; set; }
        public DbSet<UserDistricts> UserDistricts { get; set; }

        // Auditoría
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Clean Architecture entities (v2 API)
            ConfigureCleanArchitectureEntities(modelBuilder);
            
            // Configure legacy entities (v1 API - maintain compatibility)
            ConfigureLegacyEntities(modelBuilder);
        }

        private void ConfigureCleanArchitectureEntities(ModelBuilder modelBuilder)
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
        {
            // Configure Clean Architecture User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("DomainUsers"); // Different table to avoid conflicts
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

            // Configure other Clean Architecture entities with separate tables
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("DomainContacts");
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
                entity.ToTable("DomainVisits");
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
                entity.ToTable("DomainRoles");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("DomainUserRoles");
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
                entity.ToTable("DomainChurches");
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
                entity.ToTable("DomainDistricts");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }

        private void ConfigureLegacyEntities(ModelBuilder modelBuilder)
        {
            // Configure legacy entities with proper relationships
            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Phone).IsUnique();
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.ToTable("Contacts");
                entity.HasKey(e => e.ContactId);
            });

            modelBuilder.Entity<Visits>(entity =>
            {
                entity.ToTable("Visits");
                entity.HasKey(e => e.VisitId);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.RoleId);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            modelBuilder.Entity<Districts>(entity =>
            {
                entity.ToTable("Districts");
                entity.HasKey(e => e.DistrictId);
                entity.HasIndex(e => e.DistrictName).IsUnique();
            });

            modelBuilder.Entity<Visitapp.Models.Church>(entity =>
            {
                entity.ToTable("Churches");
                entity.HasKey(e => e.ChurchId);
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(e => new { e.UserId, e.RoleId });
            });

            // Configure other legacy entities
            modelBuilder.Entity<Notifications>().HasKey(e => e.NotificationId);
            modelBuilder.Entity<Temas>().HasKey(e => e.TemaId);
            modelBuilder.Entity<PreguntasClaves>().HasKey(e => e.PreguntaId);
            modelBuilder.Entity<UserDistricts>().HasKey(e => new { e.UserId, e.DistrictId });

            // Configure legacy relationships
            modelBuilder.Entity<Contacts>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visits>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visits>()
                .HasOne(v => v.Contact)
                .WithMany()
                .HasForeignKey(v => v.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notifications>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visitapp.Models.Church>()
                .HasOne(c => c.District)
                .WithMany()
                .HasForeignKey(c => c.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Church)
                .WithMany()
                .HasForeignKey(u => u.ChurchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}