using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Raf.Models;

namespace Raf.Data;

public class RafDbContext : IdentityDbContext<Kullanici>
{
    public RafDbContext(DbContextOptions<RafDbContext> options) : base(options)
    {
    }

    public DbSet<Icerik> Icerikler => Set<Icerik>();
    public DbSet<Tur> Turler => Set<Tur>();
    public DbSet<IcerikTuru> IcerikTurleri => Set<IcerikTuru>();
    public DbSet<Inceleme> Incelemeler => Set<Inceleme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Kullanici>().ToTable("Kullanicilar");

        modelBuilder.Entity<Icerik>(e =>
        {
            e.Property(i => i.MedyaTuru).HasConversion<string>().HasMaxLength(10);
            e.Property(i => i.HariciKaynak).HasConversion<string>().HasMaxLength(10);

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

        modelBuilder.Entity<Inceleme>(e =>
        {
            e.HasIndex(i => new { i.KullaniciId, i.IcerikId }).IsUnique();

            e.HasOne(i => i.Kullanici)
                .WithMany(k => k.Incelemeler)
                .HasForeignKey(i => i.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(i => i.Icerik)
                .WithMany(ic => ic.Incelemeler)
                .HasForeignKey(i => i.IcerikId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedVerileri.Ekle(modelBuilder);
    }
}
