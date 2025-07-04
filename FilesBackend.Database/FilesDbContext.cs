using FilesBackend.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FilesBackend.Database;

public class FilesDbContext(DbContextOptions<FilesDbContext> options) : DbContext(options)
{
    public DbSet<FileEntity> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileEntity>()
            .HasIndex(f => f.FileName)
            .IsUnique();
    }
}