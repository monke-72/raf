using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Raf.Models;

namespace Raf.Data;

public static class SeedVerileri
{
    public static void Ekle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            RolOlustur("1", Roller.Yonetici),
            RolOlustur("2", Roller.Moderator),
            RolOlustur("3", Roller.Kullanici)
        );

        modelBuilder.Entity<Tur>().HasData(
            new Tur { Id = 1, Ad = "Dram" },
            new Tur { Id = 2, Ad = "Bilim Kurgu" },
            new Tur { Id = 3, Ad = "Animasyon" },
            new Tur { Id = 4, Ad = "Fantastik" },
            new Tur { Id = 5, Ad = "Aksiyon" },
            new Tur { Id = 6, Ad = "Macera" },
            new Tur { Id = 7, Ad = "Rol Yapma (RPG)" },
            new Tur { Id = 8, Ad = "Platform" },
            new Tur { Id = 9, Ad = "Simülasyon" },
            new Tur { Id = 10, Ad = "Rock" },
            new Tur { Id = 11, Ad = "Pop" },
            new Tur { Id = 12, Ad = "Alternatif" }
        );

        modelBuilder.Entity<Icerik>().HasData(
            new Icerik
            {
                Id = 1, Baslik = "Esaretin Bedeli", MedyaTuru = MedyaTuru.Film, CikisYili = 1994,
                Aciklama = "Haksız yere ömür boyu hapse mahkûm edilen bir bankacının, yıllar süren " +
                           "hapis hayatı boyunca umudunu ve dostluğunu korumasını anlatan film."
            },
            new Icerik
            {
                Id = 2, Baslik = "Başlangıç", MedyaTuru = MedyaTuru.Film, CikisYili = 2010,
                Aciklama = "İnsanların rüyalarına girerek fikir çalan bir ekibin, bu kez bir fikri " +
                           "birinin zihnine yerleştirmeye çalıştığı bilim kurgu filmi."
            },
            new Icerik
            {
                Id = 3, Baslik = "Ruhların Kaçışı", MedyaTuru = MedyaTuru.Film, CikisYili = 2001,
                Aciklama = "Ruhlar dünyasına sürüklenen küçük bir kızın ailesini kurtarmak için " +
                           "verdiği mücadeleyi anlatan animasyon filmi."
            },
            new Icerik
            {
                Id = 4, Baslik = "The Witcher 3: Wild Hunt", MedyaTuru = MedyaTuru.Oyun, CikisYili = 2015,
                Aciklama = "Canavar avcısı Geralt'ın, evlatlığı Ciri'yi aradığı açık dünya rol yapma oyunu."
            },
            new Icerik
            {
                Id = 5, Baslik = "Hollow Knight", MedyaTuru = MedyaTuru.Oyun, CikisYili = 2017,
                Aciklama = "Böceklerin yaşadığı terk edilmiş bir yeraltı krallığında geçen, " +
                           "keşif odaklı iki boyutlu aksiyon platform oyunu."
            },
            new Icerik
            {
                Id = 6, Baslik = "Stardew Valley", MedyaTuru = MedyaTuru.Oyun, CikisYili = 2016,
                Aciklama = "Dedesinden kalan çiftliği yeniden canlandıran bir karakterin hikâyesini " +
                           "anlatan çiftlik ve yaşam simülasyonu oyunu."
            },
            new Icerik
            {
                Id = 7, Baslik = "Pink Floyd – The Dark Side of the Moon", MedyaTuru = MedyaTuru.Album,
                CikisYili = 1973,
                Aciklama = "Zaman, para ve akıl sağlığı gibi temaları kesintisiz bir bütün olarak işleyen " +
                           "progresif rock albümü."
            },
            new Icerik
            {
                Id = 8, Baslik = "Radiohead – OK Computer", MedyaTuru = MedyaTuru.Album, CikisYili = 1997,
                Aciklama = "Teknoloji ve modern yaşamın yabancılaştırıcı etkisini işleyen alternatif rock albümü."
            },
            new Icerik
            {
                Id = 9, Baslik = "Sezen Aksu – Gülümse", MedyaTuru = MedyaTuru.Album, CikisYili = 1991,
                Aciklama = "Sanatçının en çok bilinen şarkılarından bazılarını içeren pop albümü."
            }
        );

        modelBuilder.Entity<IcerikTuru>().HasData(
            new IcerikTuru { IcerikId = 1, TurId = 1 },
            new IcerikTuru { IcerikId = 2, TurId = 2 },
            new IcerikTuru { IcerikId = 2, TurId = 5 },
            new IcerikTuru { IcerikId = 3, TurId = 3 },
            new IcerikTuru { IcerikId = 3, TurId = 4 },
            new IcerikTuru { IcerikId = 4, TurId = 7 },
            new IcerikTuru { IcerikId = 4, TurId = 6 },
            new IcerikTuru { IcerikId = 5, TurId = 8 },
            new IcerikTuru { IcerikId = 5, TurId = 5 },
            new IcerikTuru { IcerikId = 6, TurId = 9 },
            new IcerikTuru { IcerikId = 7, TurId = 10 },
            new IcerikTuru { IcerikId = 8, TurId = 10 },
            new IcerikTuru { IcerikId = 8, TurId = 12 },
            new IcerikTuru { IcerikId = 9, TurId = 11 }
        );
    }

    private static IdentityRole RolOlustur(string id, string ad) => new()
    {
        Id = id,
        Name = ad,
        NormalizedName = ad.ToUpperInvariant(),
        ConcurrencyStamp = id
    };
}
