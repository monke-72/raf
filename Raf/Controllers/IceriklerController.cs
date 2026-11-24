using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Models;

namespace Raf.Controllers;

public class IceriklerController : Controller
{
    private readonly RafDbContext _context;
    private readonly UserManager<Kullanici> _userManager;

    public IceriklerController(RafDbContext context, UserManager<Kullanici> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(MedyaTuru? tur)
    {
        var sorgu = _context.Icerikler.AsNoTracking();

        if (tur.HasValue)
        {
            sorgu = sorgu.Where(i => i.MedyaTuru == tur.Value);
        }

        var icerikler = await sorgu
            .OrderBy(i => i.Baslik)
            .ToListAsync();

        ViewData["SeciliTur"] = tur;
        return View(icerikler);
    }

    public async Task<IActionResult> Detay(int id)
    {
        var icerik = await _context.Icerikler
            .AsNoTracking()
            .Include(i => i.IcerikTurleri)
                .ThenInclude(it => it.Tur)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (icerik == null)
        {
            return NotFound();
        }

        var incelemeler = await _context.Incelemeler
            .AsNoTracking()
            .Include(i => i.Kullanici)
            .Where(i => i.IcerikId == id)
            .OrderByDescending(i => i.Tarih)
            .ToListAsync();

        var model = new IcerikDetayModel
        {
            Icerik = icerik,
            Incelemeler = incelemeler,
            OrtalamaPuan = incelemeler.Count > 0 ? incelemeler.Average(i => i.Puan) : null,
            Form = new IncelemeFormModel { IcerikId = id }
        };

        var kullaniciId = _userManager.GetUserId(User);
        var kendiIncelemesi = incelemeler.FirstOrDefault(i => i.KullaniciId == kullaniciId);
        if (kendiIncelemesi != null)
        {
            model.KendiIncelemesiVar = true;
            model.Form.Puan = kendiIncelemesi.Puan;
            model.Form.Metin = kendiIncelemesi.Metin;
        }

        return View(model);
    }
}
