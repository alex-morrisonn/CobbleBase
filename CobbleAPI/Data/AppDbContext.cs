using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CobbleAPI.Data
{
    // Account entity
    public class Account
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // DbContext for Account entity
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<Account> Accounts => Set<Account>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // You can add additional entity configurations here
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();
                
            base.OnModelCreating(modelBuilder);
        }
    }
}