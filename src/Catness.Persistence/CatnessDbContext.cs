using Catness.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Catness.Persistence;

public class CatnessDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public CatnessDbContext() { }

    public CatnessDbContext(DbContextOptions<CatnessDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // for migrations
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=catnessdb;Command Timeout=60;Timeout=60;Persist Security Info=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(userEntity => userEntity.Id);
        });
    }
}