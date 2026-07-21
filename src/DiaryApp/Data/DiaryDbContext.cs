namespace DiaryApp.Data;

using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

public class DiaryDbContext : DbContext
{
    public DiaryDbContext(DbContextOptions<DiaryDbContext> options) : base(options) { }

    public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<DiaryImage> DiaryImages => Set<DiaryImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        modelBuilder.Entity<DiaryEntry>()
            .HasMany(e => e.Tags)
            .WithMany(t => t.Entries);

        modelBuilder.Entity<DiaryImage>()
            .HasOne(i => i.DiaryEntry)
            .WithMany(e => e.Images)
            .HasForeignKey(i => i.DiaryEntryId);
    }
}
