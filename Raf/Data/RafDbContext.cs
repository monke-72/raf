using Microsoft.EntityFrameworkCore;
using Raf.Models;

namespace Raf.Data;

public class RafDbContext : DbContext
{
    public RafDbContext(DbContextOptions<RafDbContext> options) : base(options)
    {
    }

    public DbSet<Icerik> Icerikler => Set<Icerik>();
    public DbSet<Tur> Turler => Set<Tur>();
    public DbSet<IcerikTuru> IcerikTurleri => Set<IcerikTuru>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Icerik>(e =>
        {
            // Enum'lar veritabanında sayı yerine okunabilir metin olarak saklanır.
            e.Property(i => i.MedyaTuru).HasConversion<string>().HasMaxLength(10);
            e.Property(i => i.HariciKaynak).HasConversion<string>().HasMaxLength(10);

            // Aynı harici içerik iki kez eklenemez.
            e.HasIndex(i => new { i.HariciKaynak, i.HariciId }).IsUnique();
        });

        modelBuilder.Entity<Tur>()
            .HasIndex(t => t.Ad)
            .IsUnique();

        modelBuilder.Entity<IcerikTuru>(e =>
        {
            e.HasKey(it => new { it.IcerikId, it.TurId });

            e.HasOne(it => it.Icerik)
                .WithMany(i => i.IcerikTurleri)
                .HasForeignKey(it => it.IcerikId);

            e.HasOne(it => it.Tur)
                .WithMany(t => t.IcerikTurleri)
                .HasForeignKey(it => it.TurId);
        });

        SeedVerileri.Ekle(modelBuilder);
    }
}
