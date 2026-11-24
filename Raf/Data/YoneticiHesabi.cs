using Microsoft.AspNetCore.Identity;
using Raf.Models;

namespace Raf.Data;

public static class YoneticiHesabi
{
    public static async Task OlusturAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Kullanici>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Kullanici>>();

        var email = config["Yonetici:Email"];
        var sifre = config["Yonetici:Sifre"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sifre))
        {
            return;
        }

        if (await userManager.FindByEmailAsync(email) != null)
        {
            return;
        }

        var yonetici = new Kullanici
        {
            UserName = "yonetici",
            Email = email,
            EmailConfirmed = true
        };

        var sonuc = await userManager.CreateAsync(yonetici, sifre);
        if (!sonuc.Succeeded)
        {
            logger.LogWarning("Yönetici hesabı oluşturulamadı: {Hatalar}",
                string.Join(", ", sonuc.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(yonetici, Roller.Yonetici);
    }
}
