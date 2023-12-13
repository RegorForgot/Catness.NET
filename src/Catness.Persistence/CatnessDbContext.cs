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
        optionsBuilder.UseNpgsql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(userEntity => userEntity.Id);
        });
    }
}