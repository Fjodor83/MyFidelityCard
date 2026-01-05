using FidelityCard.Domain.Entities;
using FidelityCard.Srv.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FidelityCard.Srv.Data;

public class FidelityCardDbContext : DbContext
{
    public FidelityCardDbContext(DbContextOptions<FidelityCardDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Fidelity> Fidelity { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applica tutte le configurazioni Fluent API
        modelBuilder.ApplyConfiguration(new FidelityConfiguration());
    }
}
