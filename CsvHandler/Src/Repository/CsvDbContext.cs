using CsvHandler.Entity;
using Microsoft.EntityFrameworkCore;

namespace CsvHandler.Repository;

public class CsvDbContext : DbContext
{
    public CsvDbContext(DbContextOptions<CsvDbContext> options) 
        : base(options)
    { }
    
    public DbSet<CsvFileEntity> Files { get; set; } = null!;
    public DbSet<ValuesEntity> Values { get; set; } = null!;
    public DbSet<ResultsEntity> Results { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CsvFileEntity>()
            .HasMany(f => f.Values)
            .WithOne(v => v.CsvFileEntity)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CsvFileEntity>()
            .HasOne(f => f.Results)
            .WithOne(v => v.CsvFileEntity)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
