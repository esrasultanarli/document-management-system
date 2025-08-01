using Microsoft.EntityFrameworkCore;
using DocumentManagementSystem.Models;

namespace DocumentManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Document entity configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.FileType).IsRequired();
                entity.Property(e => e.Content).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Summary).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Keywords).HasColumnType("nvarchar(max)");
                entity.Property(e => e.UploadDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.LastModified).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.ProcessingStatus).HasDefaultValue("Pending");
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.LastLoginDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Role).HasDefaultValue("User");

                // Unique constraints
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                CreatedDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                IsActive = true
            });

            // Sample documents
            modelBuilder.Entity<Document>().HasData(new Document
            {
                Id = 1,
                Title = "Örnek Doküman",
                FileName = "ornek_dokuman.txt",
                FilePath = "/uploads/ornek_dokuman.txt",
                FileType = "txt",
                FileSize = 1024,
                Content = "Bu bir örnek doküman içeriğidir.",
                Summary = "Örnek doküman özeti",
                Keywords = "örnek, doküman, test",
                UploadDate = DateTime.Now,
                LastModified = DateTime.Now,
                UploadedBy = "admin",
                IsProcessed = true,
                ProcessingStatus = "Completed"
            });
        }
    }
}