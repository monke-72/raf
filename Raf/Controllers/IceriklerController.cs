using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Models;

namespace Raf.Controllers;

public class IceriklerController : Controller
{
    private readonly RafDbContext _context;

    public IceriklerController(RafDbContext context)
    {
        _context = context;
    }

    // GET: /Icerikler  veya  /Icerikler?tur=Film
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

    // GET: /Icerikler/Detay/5
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

        return View(icerik);
    }
}
