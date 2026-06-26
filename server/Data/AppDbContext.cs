using Microsoft.EntityFrameworkCore;
using RoboSchool.Models;

namespace RoboSchool.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationRecord> Applications => Set<ApplicationRecord>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<Trainer> Trainers => Set<Trainer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationRecord>(entity =>
        {
            entity.ToTable("applications");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(120).IsRequired();
            entity.Property(a => a.Phone).HasMaxLength(30).IsRequired();
            entity.Property(a => a.Email).HasMaxLength(120).IsRequired();
            entity.Property(a => a.PackageCode).HasMaxLength(20);
            entity.HasIndex(a => a.CreatedAt);
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.ToTable("packages");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code).HasMaxLength(20).IsRequired();
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Name).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.ToTable("trainers");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Slug).HasMaxLength(40).IsRequired();
            entity.HasIndex(t => t.Slug).IsUnique();
            entity.Property(t => t.Name).HasMaxLength(120).IsRequired();
            entity.Property(t => t.Role).HasMaxLength(120).IsRequired();
            entity.Property(t => t.PhotoUrl).HasMaxLength(500).IsRequired();
            entity.Property(t => t.Bio).HasMaxLength(2000).IsRequired();
        });
    }
}
