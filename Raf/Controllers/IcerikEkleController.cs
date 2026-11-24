using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Models;
using Raf.Services;

namespace Raf.Controllers;

public class IcerikEkleController : Controller
{
    private readonly RafDbContext _context;
    private readonly ITmdbService _tmdb;
    private readonly IRawgService _rawg;

    public IcerikEkleController(RafDbContext context, ITmdbService tmdb, IRawgService rawg)
    {
        _context = context;
        _tmdb = tmdb;
        _rawg = rawg;
    }

    public async Task<IActionResult> Index(string? sorgu, MedyaTuru tur = MedyaTuru.Film)
    {
        ViewData["Sorgu"] = sorgu;
        ViewData["Tur"] = tur;
        ViewData["Ekliler"] = new Dictionary<string, int>();

        if (string.IsNullOrWhiteSpace(sorgu))
        {
            return View(new List<HariciIcerik>());
        }

        if (tur == MedyaTuru.Album)
        {
            ViewData["Hata"] = "Albüm araması henüz eklenmedi.";
            return View(new List<HariciIcerik>());
        }

        List<HariciIcerik> sonuclar;
        try
        {
            sonuclar = tur == MedyaTuru.Oyun
                ? await _rawg.AraAsync(sorgu)
                : await _tmdb.AraAsync(sorgu);
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
        {
            ViewData["Hata"] = "Arama yapılamadı: " + ex.Message;
            return View(new List<HariciIcerik>());
        }

        var kaynak = KaynakBul(tur);
        var idler = sonuclar.Select(s => s.HariciId).ToList();

        ViewData["Ekliler"] = await _context.Icerikler
            .Where(i => i.HariciKaynak == kaynak && idler.Contains(i.HariciId!))
            .ToDictionaryAsync(i => i.HariciId!, i => i.Id);

        return View(sonuclar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(MedyaTuru tur, string hariciId)
    {
        if (tur == MedyaTuru.Album || string.IsNullOrWhiteSpace(hariciId))
        {
            return BadRequest();
        }

        var kaynak = KaynakBul(tur);

        var mevcut = await _context.Icerikler
            .FirstOrDefaultAsync(i => i.HariciKaynak == kaynak && i.HariciId == hariciId);

        if (mevcut != null)
        {
            TempData["Mesaj"] = "Bu içerik zaten kütüphanede.";
            return RedirectToAction("Detay", "Icerikler", new { id = mevcut.Id });
        }

        HariciIcerik? veri;
        try
        {
            veri = tur == MedyaTuru.Oyun
                ? await _rawg.GetirAsync(hariciId)
                : await _tmdb.GetirAsync(hariciId);
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
        {
            TempData["Hata"] = "İçerik getirilemedi: " + ex.Message;
            return RedirectToAction(nameof(Index), new { tur });
        }

        if (veri == null)
        {
            return NotFound();
        }

        var icerik = new Icerik
        {
            Baslik = Kisalt(veri.Baslik, 200)!,
            MedyaTuru = tur,
            CikisYili = veri.CikisYili,
            Aciklama = Kisalt(veri.Aciklama, 4000),
            GorselUrl = Kisalt(veri.GorselUrl, 500),
            HariciKaynak = kaynak,
            HariciId = veri.HariciId
        };

        foreach (var turAdi in veri.Turler.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var ad = Kisalt(turAdi, 50)!;
            var turKaydi = await _context.Turler.FirstOrDefaultAsync(t => t.Ad.ToLower() == ad.ToLower())
                           ?? new Tur { Ad = ad };

            icerik.IcerikTurleri.Add(new IcerikTuru { Tur = turKaydi });
        }

        _context.Icerikler.Add(icerik);
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"\"{icerik.Baslik}\" kütüphaneye eklendi.";
        return RedirectToAction("Detay", "Icerikler", new { id = icerik.Id });
    }

    private static HariciKaynak KaynakBul(MedyaTuru tur) =>
        tur == MedyaTuru.Oyun ? HariciKaynak.Rawg : HariciKaynak.Tmdb;

    private static string? Kisalt(string? metin, int uzunluk) =>
        metin == null || metin.Length <= uzunluk ? metin : metin[..uzunluk];
}
