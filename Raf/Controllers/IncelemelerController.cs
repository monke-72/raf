using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Models;

namespace Raf.Controllers;

[Authorize]
public class IncelemelerController : Controller
{
    private readonly RafDbContext _context;
    private readonly UserManager<Kullanici> _userManager;

    public IncelemelerController(RafDbContext context, UserManager<Kullanici> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kaydet([Bind(Prefix = "Form")] IncelemeFormModel form)
    {
        if (!await _context.Icerikler.AnyAsync(i => i.Id == form.IcerikId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Hata"] = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return RedirectToAction("Detay", "Icerikler", new { id = form.IcerikId });
        }

        var kullaniciId = _userManager.GetUserId(User)!;

        var inceleme = await _context.Incelemeler
            .FirstOrDefaultAsync(i => i.KullaniciId == kullaniciId && i.IcerikId == form.IcerikId);

        if (inceleme == null)
        {
            inceleme = new Inceleme { KullaniciId = kullaniciId, IcerikId = form.IcerikId };
            _context.Incelemeler.Add(inceleme);
            TempData["Mesaj"] = "İncelemen kaydedildi.";
        }
        else
        {
            TempData["Mesaj"] = "İncelemen güncellendi.";
        }

        inceleme.Puan = form.Puan!.Value;
        inceleme.Metin = string.IsNullOrWhiteSpace(form.Metin) ? null : form.Metin.Trim();
        inceleme.Tarih = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction("Detay", "Icerikler", new { id = form.IcerikId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var inceleme = await _context.Incelemeler.FindAsync(id);
        if (inceleme == null)
        {
            return NotFound();
        }

        var sahibiMi = inceleme.KullaniciId == _userManager.GetUserId(User);
        var yetkiliMi = User.IsInRole(Roller.Moderator) || User.IsInRole(Roller.Yonetici);

        if (!sahibiMi && !yetkiliMi)
        {
            return Forbid();
        }

        _context.Incelemeler.Remove(inceleme);
        await _context.SaveChangesAsync();

        TempData["Mesaj"] = "İnceleme silindi.";
        return RedirectToAction("Detay", "Icerikler", new { id = inceleme.IcerikId });
    }
}
