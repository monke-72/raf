using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Raf.Data;
using Raf.Models;
using Raf.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Bağlantı dizesi bulunamadı. README'deki 'dotnet user-secrets set' adımını uygulayın.");
}

builder.Services.AddDbContext<RafDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters += "çğıöşüÇĞİÖŞÜ";
    })
    .AddEntityFrameworkStores<RafDbContext>()
    .AddErrorDescriber<TurkceIdentityHatalari>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Hesap/Giris";
    options.LogoutPath = "/Hesap/Cikis";
    options.AccessDeniedPath = "/Hesap/ErisimEngellendi";
});

builder.Services.AddHttpClient<ITmdbService, TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});

builder.Services.AddHttpClient<IRawgService, RawgService>(client =>
{
    client.BaseAddress = new Uri("https://api.rawg.io/api/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Raf/1.0");
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

await YoneticiHesabi.OlusturAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
