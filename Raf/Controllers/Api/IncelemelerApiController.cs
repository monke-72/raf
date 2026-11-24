using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Models;

namespace Raf.Controllers.Api;

[ApiController]
[Route("api/incelemeler")]
[Produces("application/json")]
[Tags("İncelemeler")]
public class IncelemelerApiController : ControllerBase
{
    private readonly RafDbContext _context;
    private readonly UserManager<Kullanici> _userManager;

    public IncelemelerApiController(RafDbContext context, UserManager<Kullanici> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IncelemeDto>>> Listele(int? icerikId)
    {
        var sorgu = _context.Incelemeler.AsNoTracking();

        if (icerikId.HasValue)
        {
            sorgu = sorgu.Where(i => i.IcerikId == icerikId.Value);
        }

        return await sorgu
            .OrderByDescending(i => i.Tarih)
            .Select(i => new IncelemeDto
            {
                Id = i.Id,
                IcerikId = i.IcerikId,
                IcerikBaslik = i.Icerik.Baslik,
                KullaniciAdi = i.Kullanici.UserName!,
                Puan = i.Puan,
                Metin = i.Metin,
                Tarih = i.Tarih
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncelemeDto>> Getir(int id)
    {
        var dto = await _context.Incelemeler
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new IncelemeDto
            {
                Id = i.Id,
                IcerikId = i.IcerikId,
                IcerikBaslik = i.Icerik.Baslik,
                KullaniciAdi = i.Kullanici.UserName!,
                Puan = i.Puan,
                Metin = i.Metin,
                Tarih = i.Tarih
            })
            .FirstOrDefaultAsync();

        return dto == null ? NotFound() : dto;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IncelemeDto>> Olustur(IncelemeOlusturDto dto)
    {
        var icerik = await _context.Icerikler.FindAsync(dto.IcerikId);
        if (icerik == null)
        {
            return NotFound(new { mesaj = "İçerik bulunamadı." });
        }

        var kullaniciId = _userManager.GetUserId(User)!;

        if (await _context.Incelemeler.AnyAsync(i => i.KullaniciId == kullaniciId && i.IcerikId == icerik.Id))
        {
            return Conflict(new { mesaj = "Bu içeriğe zaten bir inceleme yazdın. Güncellemek için PUT kullan." });
        }

        var inceleme = new Inceleme
        {
            KullaniciId = kullaniciId,
            IcerikId = icerik.Id,
            Puan = dto.Puan!.Value,
            Metin = string.IsNullOrWhiteSpace(dto.Metin) ? null : dto.Metin.Trim(),
            Tarih = DateTime.UtcNow
        };

        _context.Incelemeler.Add(inceleme);
        await _context.SaveChangesAsync();

        var sonuc = new IncelemeDto
        {
            Id = inceleme.Id,
            IcerikId = icerik.Id,
            IcerikBaslik = icerik.Baslik,
            KullaniciAdi = User.Identity!.Name!,
            Puan = inceleme.Puan,
            Metin = inceleme.Metin,
            Tarih = inceleme.Tarih
        };

        return CreatedAtAction(nameof(Getir), new { id = inceleme.Id }, sonuc);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Guncelle(int id, IncelemeGuncelleDto dto)
    {
        var inceleme = await _context.Incelemeler.FindAsync(id);
        if (inceleme == null)
        {
            return NotFound();
        }

        if (inceleme.KullaniciId != _userManager.GetUserId(User))
        {
            return Forbid();
        }

        inceleme.Puan = dto.Puan!.Value;
        inceleme.Metin = string.IsNullOrWhiteSpace(dto.Metin) ? null : dto.Metin.Trim();
        inceleme.Tarih = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        return NoContent();
    }
}
