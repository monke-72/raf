using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Raf.Models;

namespace Raf.Controllers;

public class HesapController : Controller
{
    private readonly UserManager<Kullanici> _userManager;
    private readonly SignInManager<Kullanici> _signInManager;

    public HesapController(UserManager<Kullanici> userManager, SignInManager<Kullanici> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Kayit() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kayit(KayitModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var kullanici = new Kullanici
        {
            UserName = model.KullaniciAdi,
            Email = model.Email
        };

        var sonuc = await _userManager.CreateAsync(kullanici, model.Sifre);
        if (!sonuc.Succeeded)
        {
            foreach (var hata in sonuc.Errors)
            {
                ModelState.AddModelError(string.Empty, hata.Description);
            }
            return View(model);
        }

        await _userManager.AddToRoleAsync(kullanici, Roller.Kullanici);
        await _signInManager.SignInAsync(kullanici, isPersistent: false);

        TempData["Mesaj"] = $"Hoş geldin, {kullanici.UserName}!";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Giris(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(GirisModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var kullanici = await _userManager.FindByEmailAsync(model.Email);
        if (kullanici != null)
        {
            var sonuc = await _signInManager.PasswordSignInAsync(
                kullanici, model.Sifre, model.BeniHatirla, lockoutOnFailure: true);

            if (sonuc.Succeeded)
            {
                return Url.IsLocalUrl(returnUrl)
                    ? Redirect(returnUrl!)
                    : RedirectToAction("Index", "Home");
            }

            if (sonuc.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Çok fazla hatalı deneme yapıldı. Lütfen birkaç dakika sonra tekrar deneyin.");
                return View(model);
            }
        }

        ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Cikis()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ErisimEngellendi() => View();
}
