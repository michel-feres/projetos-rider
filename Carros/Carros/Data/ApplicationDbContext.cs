using Carros.Models;
using Microsoft.EntityFrameworkCore;

namespace Carros.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Carro> Carros { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Fabricante>()
            .HasMany(f => f.Carros)
            .WithOne(p => p.Fabricante)
            .HasForeignKey(p => p.FabricanteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}